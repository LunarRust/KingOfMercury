using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Godot;

namespace MEC;

[ScriptPath("res://addons/GDMEC/Timing.cs")]
public partial class Timing : Node
{
	private struct ProcessIndex : IEquatable<ProcessIndex>
	{
		public Segment seg;

		public int i;

		public bool Equals(ProcessIndex other)
		{
			return seg == other.seg && i == other.i;
		}

		public override bool Equals(object other)
		{
			if (other is ProcessIndex)
			{
				return Equals((ProcessIndex)other);
			}
			return false;
		}

		public static bool operator ==(ProcessIndex a, ProcessIndex b)
		{
			return a.seg == b.seg && a.i == b.i;
		}

		public static bool operator !=(ProcessIndex a, ProcessIndex b)
		{
			return a.seg != b.seg || a.i != b.i;
		}

		public override int GetHashCode()
		{
			return (int)(seg - 2) * 715827882 + i;
		}
	}

	[Export(PropertyHint.None, "")]
	public int ProcessCoroutines;

	[Export(PropertyHint.None, "")]
	public int PhysicsProcessCoroutines;

	[Export(PropertyHint.None, "")]
	public int DeferredProcessCoroutines;

	[NonSerialized]
	public double localTime;

	[NonSerialized]
	public double deltaTime;

	public static Func<IEnumerator<double>, CoroutineHandle, IEnumerator<double>> ReplacementFunction;

	public const double WaitForOneFrame = double.NegativeInfinity;

	private static object _tmpRef;

	private ulong _currentProcessFrame;

	private ulong _currentDeferredProcessFrame;

	private int _nextProcessProcessSlot;

	private int _nextDeferredProcessProcessSlot;

	private int _nextPhysicsProcessProcessSlot;

	private int _lastProcessProcessSlot;

	private int _lastDeferredProcessProcessSlot;

	private int _lastPhysicsProcessProcessSlot;

	private double _lastProcessTime;

	private double _lastDeferredProcessTime;

	private double _physicsProcessTime;

	private double _lastPhysicsProcessTime;

	private ushort _framesSinceProcess;

	private ushort _expansions = 1;

	private byte _instanceID;

	private readonly Dictionary<CoroutineHandle, HashSet<CoroutineHandle>> _waitingTriggers = new Dictionary<CoroutineHandle, HashSet<CoroutineHandle>>();

	private readonly HashSet<CoroutineHandle> _allWaiting = new HashSet<CoroutineHandle>();

	private readonly Dictionary<CoroutineHandle, ProcessIndex> _handleToIndex = new Dictionary<CoroutineHandle, ProcessIndex>();

	private readonly Dictionary<ProcessIndex, CoroutineHandle> _indexToHandle = new Dictionary<ProcessIndex, CoroutineHandle>();

	private readonly Dictionary<CoroutineHandle, string> _processTags = new Dictionary<CoroutineHandle, string>();

	private readonly Dictionary<string, HashSet<CoroutineHandle>> _taggedProcesses = new Dictionary<string, HashSet<CoroutineHandle>>();

	private IEnumerator<double>[] ProcessProcesses = new IEnumerator<double>[256];

	private IEnumerator<double>[] DeferredProcessProcesses = new IEnumerator<double>[8];

	private IEnumerator<double>[] PhysicsProcessProcesses = new IEnumerator<double>[64];

	private bool[] ProcessPaused = new bool[256];

	private bool[] DeferredProcessPaused = new bool[8];

	private bool[] PhysicsProcessPaused = new bool[64];

	private bool[] ProcessHeld = new bool[256];

	private bool[] DeferredProcessHeld = new bool[8];

	private bool[] PhysicsProcessHeld = new bool[64];

	private const ushort FramesUntilMaintenance = 64;

	private const int ProcessArrayChunkSize = 64;

	private const int InitialBufferSizeLarge = 256;

	private const int InitialBufferSizeMedium = 64;

	private const int InitialBufferSizeSmall = 8;

	private static Timing[] ActiveInstances = new Timing[16];

	private static Timing _instance;

	public static double DeltaTime => Instance.deltaTime;

	public static Thread MainThread { get; private set; }

	public CoroutineHandle currentCoroutine { get; private set; }

	public static Timing Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = ((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull<Timing>(typeof(Timing).Name);
				if (_instance == null)
				{
					_instance = new Timing();
					_instance.Name = typeof(Timing).Name;
					_instance.CallDeferred("InitGlobalInstance");
				}
			}
			return _instance;
		}
	}

	public Timing()
	{
		InitializeInstanceID();
	}

	private void InitGlobalInstance()
	{
		((SceneTree)Engine.GetMainLoop()).Root.AddChild(this, forceReadableName: false, InternalMode.Disabled);
	}

	public override void _Ready()
	{
		base.ProcessPriority = -1;
		try
		{
			GetType().GetProperty("ProcessPhysicsPriority", BindingFlags.Instance | BindingFlags.Public).SetValue(this, -1);
		}
		catch (NullReferenceException)
		{
		}
		if (MainThread == null)
		{
			MainThread = Thread.CurrentThread;
		}
	}

	public override void _ExitTree()
	{
		if (_instanceID < ActiveInstances.Length)
		{
			ActiveInstances[_instanceID] = null;
		}
	}

	private void InitializeInstanceID()
	{
		if (ActiveInstances[_instanceID] != null)
		{
			return;
		}
		if (_instanceID == 0)
		{
			_instanceID++;
		}
		while (_instanceID <= 16)
		{
			if (_instanceID == 16)
			{
				QueueFree();
				throw new OverflowException("You are only allowed 15 different contexts for MEC to run inside at one time.");
			}
			if (ActiveInstances[_instanceID] == null)
			{
				ActiveInstances[_instanceID] = this;
				break;
			}
			_instanceID++;
		}
	}

	public override void _Process(double delta)
	{
		if (_nextProcessProcessSlot > 0)
		{
			ProcessIndex processIndex = default(ProcessIndex);
			processIndex.seg = Segment.Process;
			ProcessIndex key = processIndex;
			if (UpdateTimeValues(key.seg))
			{
				_lastProcessProcessSlot = _nextProcessProcessSlot;
			}
			key.i = 0;
			while (key.i < _lastProcessProcessSlot)
			{
				try
				{
					if (!ProcessPaused[key.i] && !ProcessHeld[key.i] && ProcessProcesses[key.i] != null && !(localTime < ProcessProcesses[key.i].Current))
					{
						currentCoroutine = _indexToHandle[key];
						if (!ProcessProcesses[key.i].MoveNext())
						{
							if (_indexToHandle.ContainsKey(key))
							{
								KillCoroutinesOnInstance(_indexToHandle[key]);
							}
						}
						else if (ProcessProcesses[key.i] != null && double.IsNaN(ProcessProcesses[key.i].Current))
						{
							if (ReplacementFunction != null)
							{
								ProcessProcesses[key.i] = ReplacementFunction(ProcessProcesses[key.i], _indexToHandle[key]);
								ReplacementFunction = null;
							}
							key.i--;
						}
					}
				}
				catch (Exception ex)
				{
					GD.PrintErr(ex);
				}
				key.i++;
			}
		}
		currentCoroutine = default(CoroutineHandle);
		if (++_framesSinceProcess > 64)
		{
			_framesSinceProcess = 0;
			RemoveUnused();
		}
		CallDeferred("_DeferredProcess");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_nextPhysicsProcessProcessSlot <= 0)
		{
			return;
		}
		ProcessIndex processIndex = default(ProcessIndex);
		processIndex.seg = Segment.PhysicsProcess;
		ProcessIndex key = processIndex;
		if (UpdateTimeValues(key.seg))
		{
			_lastPhysicsProcessProcessSlot = _nextPhysicsProcessProcessSlot;
		}
		key.i = 0;
		while (key.i < _lastPhysicsProcessProcessSlot)
		{
			try
			{
				if (!PhysicsProcessPaused[key.i] && !PhysicsProcessHeld[key.i] && PhysicsProcessProcesses[key.i] != null && !(localTime < PhysicsProcessProcesses[key.i].Current))
				{
					currentCoroutine = _indexToHandle[key];
					if (!PhysicsProcessProcesses[key.i].MoveNext())
					{
						if (_indexToHandle.ContainsKey(key))
						{
							KillCoroutinesOnInstance(_indexToHandle[key]);
						}
					}
					else if (PhysicsProcessProcesses[key.i] != null && double.IsNaN(PhysicsProcessProcesses[key.i].Current))
					{
						if (ReplacementFunction != null)
						{
							PhysicsProcessProcesses[key.i] = ReplacementFunction(PhysicsProcessProcesses[key.i], _indexToHandle[key]);
							ReplacementFunction = null;
						}
						key.i--;
					}
				}
			}
			catch (Exception ex)
			{
				GD.PrintErr(ex);
			}
			key.i++;
		}
		currentCoroutine = default(CoroutineHandle);
	}

	private void _DeferredProcess()
	{
		if (_nextDeferredProcessProcessSlot <= 0)
		{
			return;
		}
		ProcessIndex processIndex = default(ProcessIndex);
		processIndex.seg = Segment.DeferredProcess;
		ProcessIndex key = processIndex;
		if (UpdateTimeValues(key.seg))
		{
			_lastDeferredProcessProcessSlot = _nextDeferredProcessProcessSlot;
		}
		key.i = 0;
		while (key.i < _lastDeferredProcessProcessSlot)
		{
			try
			{
				if (!DeferredProcessPaused[key.i] && !DeferredProcessHeld[key.i] && DeferredProcessProcesses[key.i] != null && !(localTime < DeferredProcessProcesses[key.i].Current))
				{
					currentCoroutine = _indexToHandle[key];
					if (!DeferredProcessProcesses[key.i].MoveNext())
					{
						if (_indexToHandle.ContainsKey(key))
						{
							KillCoroutinesOnInstance(_indexToHandle[key]);
						}
					}
					else if (DeferredProcessProcesses[key.i] != null && double.IsNaN(DeferredProcessProcesses[key.i].Current))
					{
						if (ReplacementFunction != null)
						{
							DeferredProcessProcesses[key.i] = ReplacementFunction(DeferredProcessProcesses[key.i], _indexToHandle[key]);
							ReplacementFunction = null;
						}
						key.i--;
					}
				}
			}
			catch (Exception ex)
			{
				GD.PrintErr(ex);
			}
			key.i++;
		}
		currentCoroutine = default(CoroutineHandle);
	}

	private void RemoveUnused()
	{
		Dictionary<CoroutineHandle, HashSet<CoroutineHandle>>.Enumerator enumerator = _waitingTriggers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value.Count == 0)
			{
				_waitingTriggers.Remove(enumerator.Current.Key);
				enumerator = _waitingTriggers.GetEnumerator();
			}
			else if (_handleToIndex.ContainsKey(enumerator.Current.Key) && CoindexIsNull(_handleToIndex[enumerator.Current.Key]))
			{
				CloseWaitingProcess(enumerator.Current.Key);
				enumerator = _waitingTriggers.GetEnumerator();
			}
		}
		ProcessIndex key = default(ProcessIndex);
		ProcessIndex processIndex = default(ProcessIndex);
		key.seg = (processIndex.seg = Segment.Process);
		key.i = (processIndex.i = 0);
		while (key.i < _nextProcessProcessSlot)
		{
			if (ProcessProcesses[key.i] != null)
			{
				if (key.i != processIndex.i)
				{
					ProcessProcesses[processIndex.i] = ProcessProcesses[key.i];
					ProcessPaused[processIndex.i] = ProcessPaused[key.i];
					ProcessHeld[processIndex.i] = ProcessHeld[key.i];
					if (_indexToHandle.ContainsKey(processIndex))
					{
						RemoveTag(_indexToHandle[processIndex]);
						_handleToIndex.Remove(_indexToHandle[processIndex]);
						_indexToHandle.Remove(processIndex);
					}
					_handleToIndex[_indexToHandle[key]] = processIndex;
					_indexToHandle.Add(processIndex, _indexToHandle[key]);
					_indexToHandle.Remove(key);
				}
				processIndex.i++;
			}
			key.i++;
		}
		key.i = processIndex.i;
		while (key.i < _nextProcessProcessSlot)
		{
			ProcessProcesses[key.i] = null;
			ProcessPaused[key.i] = false;
			ProcessHeld[key.i] = false;
			if (_indexToHandle.ContainsKey(key))
			{
				RemoveTag(_indexToHandle[key]);
				_handleToIndex.Remove(_indexToHandle[key]);
				_indexToHandle.Remove(key);
			}
			key.i++;
		}
		_lastProcessProcessSlot -= _nextProcessProcessSlot - processIndex.i;
		ProcessCoroutines = (_nextProcessProcessSlot = processIndex.i);
		key.seg = (processIndex.seg = Segment.PhysicsProcess);
		key.i = (processIndex.i = 0);
		while (key.i < _nextPhysicsProcessProcessSlot)
		{
			if (PhysicsProcessProcesses[key.i] != null)
			{
				if (key.i != processIndex.i)
				{
					PhysicsProcessProcesses[processIndex.i] = PhysicsProcessProcesses[key.i];
					PhysicsProcessPaused[processIndex.i] = PhysicsProcessPaused[key.i];
					PhysicsProcessHeld[processIndex.i] = PhysicsProcessHeld[key.i];
					if (_indexToHandle.ContainsKey(processIndex))
					{
						RemoveTag(_indexToHandle[processIndex]);
						_handleToIndex.Remove(_indexToHandle[processIndex]);
						_indexToHandle.Remove(processIndex);
					}
					_handleToIndex[_indexToHandle[key]] = processIndex;
					_indexToHandle.Add(processIndex, _indexToHandle[key]);
					_indexToHandle.Remove(key);
				}
				processIndex.i++;
			}
			key.i++;
		}
		key.i = processIndex.i;
		while (key.i < _nextPhysicsProcessProcessSlot)
		{
			PhysicsProcessProcesses[key.i] = null;
			PhysicsProcessPaused[key.i] = false;
			PhysicsProcessHeld[key.i] = false;
			if (_indexToHandle.ContainsKey(key))
			{
				RemoveTag(_indexToHandle[key]);
				_handleToIndex.Remove(_indexToHandle[key]);
				_indexToHandle.Remove(key);
			}
			key.i++;
		}
		_lastPhysicsProcessProcessSlot -= _nextPhysicsProcessProcessSlot - processIndex.i;
		PhysicsProcessCoroutines = (_nextPhysicsProcessProcessSlot = processIndex.i);
		key.seg = (processIndex.seg = Segment.DeferredProcess);
		key.i = (processIndex.i = 0);
		while (key.i < _nextDeferredProcessProcessSlot)
		{
			if (DeferredProcessProcesses[key.i] != null)
			{
				if (key.i != processIndex.i)
				{
					DeferredProcessProcesses[processIndex.i] = DeferredProcessProcesses[key.i];
					DeferredProcessPaused[processIndex.i] = DeferredProcessPaused[key.i];
					DeferredProcessHeld[processIndex.i] = DeferredProcessHeld[key.i];
					if (_indexToHandle.ContainsKey(processIndex))
					{
						RemoveTag(_indexToHandle[processIndex]);
						_handleToIndex.Remove(_indexToHandle[processIndex]);
						_indexToHandle.Remove(processIndex);
					}
					_handleToIndex[_indexToHandle[key]] = processIndex;
					_indexToHandle.Add(processIndex, _indexToHandle[key]);
					_indexToHandle.Remove(key);
				}
				processIndex.i++;
			}
			key.i++;
		}
		key.i = processIndex.i;
		while (key.i < _nextDeferredProcessProcessSlot)
		{
			DeferredProcessProcesses[key.i] = null;
			DeferredProcessPaused[key.i] = false;
			DeferredProcessHeld[key.i] = false;
			if (_indexToHandle.ContainsKey(key))
			{
				RemoveTag(_indexToHandle[key]);
				_handleToIndex.Remove(_indexToHandle[key]);
				_indexToHandle.Remove(key);
			}
			key.i++;
		}
		_lastDeferredProcessProcessSlot -= _nextDeferredProcessProcessSlot - processIndex.i;
		DeferredProcessCoroutines = (_nextDeferredProcessProcessSlot = processIndex.i);
	}

	public static CoroutineHandle RunCoroutine(IEnumerator<double> coroutine)
	{
		return (coroutine == null) ? default(CoroutineHandle) : Instance.RunCoroutineInternal(coroutine, Segment.Process, null, new CoroutineHandle(Instance._instanceID), prewarm: true);
	}

	public static CoroutineHandle RunCoroutine(IEnumerator<double> coroutine, string tag)
	{
		return (coroutine == null) ? default(CoroutineHandle) : Instance.RunCoroutineInternal(coroutine, Segment.Process, tag, new CoroutineHandle(Instance._instanceID), prewarm: true);
	}

	public static CoroutineHandle RunCoroutine(IEnumerator<double> coroutine, Segment segment)
	{
		return (coroutine == null) ? default(CoroutineHandle) : Instance.RunCoroutineInternal(coroutine, segment, null, new CoroutineHandle(Instance._instanceID), prewarm: true);
	}

	public static CoroutineHandle RunCoroutine(IEnumerator<double> coroutine, Segment segment, string tag)
	{
		return (coroutine == null) ? default(CoroutineHandle) : Instance.RunCoroutineInternal(coroutine, segment, tag, new CoroutineHandle(Instance._instanceID), prewarm: true);
	}

	public CoroutineHandle RunCoroutineOnInstance(IEnumerator<double> coroutine)
	{
		return (coroutine == null) ? default(CoroutineHandle) : RunCoroutineInternal(coroutine, Segment.Process, null, new CoroutineHandle(_instanceID), prewarm: true);
	}

	public CoroutineHandle RunCoroutineOnInstance(IEnumerator<double> coroutine, string tag)
	{
		return (coroutine == null) ? default(CoroutineHandle) : RunCoroutineInternal(coroutine, Segment.Process, tag, new CoroutineHandle(_instanceID), prewarm: true);
	}

	public CoroutineHandle RunCoroutineOnInstance(IEnumerator<double> coroutine, Segment segment)
	{
		return (coroutine == null) ? default(CoroutineHandle) : RunCoroutineInternal(coroutine, segment, null, new CoroutineHandle(_instanceID), prewarm: true);
	}

	public CoroutineHandle RunCoroutineOnInstance(IEnumerator<double> coroutine, Segment segment, string tag)
	{
		return (coroutine == null) ? default(CoroutineHandle) : RunCoroutineInternal(coroutine, segment, tag, new CoroutineHandle(_instanceID), prewarm: true);
	}

	private CoroutineHandle RunCoroutineInternal(IEnumerator<double> coroutine, Segment segment, string tag, CoroutineHandle handle, bool prewarm)
	{
		ProcessIndex processIndex = default(ProcessIndex);
		processIndex.seg = segment;
		ProcessIndex processIndex2 = processIndex;
		if (_handleToIndex.ContainsKey(handle))
		{
			_indexToHandle.Remove(_handleToIndex[handle]);
			_handleToIndex.Remove(handle);
		}
		double num = localTime;
		double num2 = deltaTime;
		CoroutineHandle coroutineHandle = currentCoroutine;
		currentCoroutine = handle;
		switch (segment)
		{
		case Segment.Process:
			if (_nextProcessProcessSlot >= ProcessProcesses.Length)
			{
				IEnumerator<double>[] processProcesses = ProcessProcesses;
				bool[] processPaused = ProcessPaused;
				bool[] processHeld = ProcessHeld;
				ProcessProcesses = new IEnumerator<double>[ProcessProcesses.Length + 64 * _expansions++];
				ProcessPaused = new bool[ProcessProcesses.Length];
				ProcessHeld = new bool[ProcessProcesses.Length];
				for (int j = 0; j < processProcesses.Length; j++)
				{
					ProcessProcesses[j] = processProcesses[j];
					ProcessPaused[j] = processPaused[j];
					ProcessHeld[j] = processHeld[j];
				}
			}
			if (UpdateTimeValues(processIndex2.seg))
			{
				_lastProcessProcessSlot = _nextProcessProcessSlot;
			}
			processIndex2.i = _nextProcessProcessSlot++;
			ProcessProcesses[processIndex2.i] = coroutine;
			if (tag != null)
			{
				AddTag(tag, handle);
			}
			_indexToHandle.Add(processIndex2, handle);
			_handleToIndex.Add(handle, processIndex2);
			while (prewarm)
			{
				if (!ProcessProcesses[processIndex2.i].MoveNext())
				{
					if (_indexToHandle.ContainsKey(processIndex2))
					{
						KillCoroutinesOnInstance(_indexToHandle[processIndex2]);
					}
					prewarm = false;
				}
				else if (ProcessProcesses[processIndex2.i] != null && double.IsNaN(ProcessProcesses[processIndex2.i].Current))
				{
					if (ReplacementFunction != null)
					{
						ProcessProcesses[processIndex2.i] = ReplacementFunction(ProcessProcesses[processIndex2.i], _indexToHandle[processIndex2]);
						ReplacementFunction = null;
					}
					prewarm = !ProcessPaused[processIndex2.i] && !ProcessHeld[processIndex2.i];
				}
				else
				{
					prewarm = false;
				}
			}
			break;
		case Segment.PhysicsProcess:
			if (_nextPhysicsProcessProcessSlot >= PhysicsProcessProcesses.Length)
			{
				IEnumerator<double>[] physicsProcessProcesses = PhysicsProcessProcesses;
				bool[] physicsProcessPaused = PhysicsProcessPaused;
				bool[] physicsProcessHeld = PhysicsProcessHeld;
				PhysicsProcessProcesses = new IEnumerator<double>[PhysicsProcessProcesses.Length + 64 * _expansions++];
				PhysicsProcessPaused = new bool[PhysicsProcessProcesses.Length];
				PhysicsProcessHeld = new bool[PhysicsProcessProcesses.Length];
				for (int k = 0; k < physicsProcessProcesses.Length; k++)
				{
					PhysicsProcessProcesses[k] = physicsProcessProcesses[k];
					PhysicsProcessPaused[k] = physicsProcessPaused[k];
					PhysicsProcessHeld[k] = physicsProcessHeld[k];
				}
			}
			if (UpdateTimeValues(processIndex2.seg))
			{
				_lastPhysicsProcessProcessSlot = _nextPhysicsProcessProcessSlot;
			}
			processIndex2.i = _nextPhysicsProcessProcessSlot++;
			PhysicsProcessProcesses[processIndex2.i] = coroutine;
			if (tag != null)
			{
				AddTag(tag, handle);
			}
			_indexToHandle.Add(processIndex2, handle);
			_handleToIndex.Add(handle, processIndex2);
			while (prewarm)
			{
				if (!PhysicsProcessProcesses[processIndex2.i].MoveNext())
				{
					if (_indexToHandle.ContainsKey(processIndex2))
					{
						KillCoroutinesOnInstance(_indexToHandle[processIndex2]);
					}
					prewarm = false;
				}
				else if (PhysicsProcessProcesses[processIndex2.i] != null && double.IsNaN(PhysicsProcessProcesses[processIndex2.i].Current))
				{
					if (ReplacementFunction != null)
					{
						PhysicsProcessProcesses[processIndex2.i] = ReplacementFunction(PhysicsProcessProcesses[processIndex2.i], _indexToHandle[processIndex2]);
						ReplacementFunction = null;
					}
					prewarm = !PhysicsProcessPaused[processIndex2.i] && !PhysicsProcessHeld[processIndex2.i];
				}
				else
				{
					prewarm = false;
				}
			}
			break;
		case Segment.DeferredProcess:
			if (_nextDeferredProcessProcessSlot >= DeferredProcessProcesses.Length)
			{
				IEnumerator<double>[] deferredProcessProcesses = DeferredProcessProcesses;
				bool[] deferredProcessPaused = DeferredProcessPaused;
				bool[] deferredProcessHeld = DeferredProcessHeld;
				DeferredProcessProcesses = new IEnumerator<double>[DeferredProcessProcesses.Length + 64 * _expansions++];
				DeferredProcessPaused = new bool[DeferredProcessProcesses.Length];
				DeferredProcessHeld = new bool[DeferredProcessProcesses.Length];
				for (int i = 0; i < deferredProcessProcesses.Length; i++)
				{
					DeferredProcessProcesses[i] = deferredProcessProcesses[i];
					DeferredProcessPaused[i] = deferredProcessPaused[i];
					DeferredProcessHeld[i] = deferredProcessHeld[i];
				}
			}
			if (UpdateTimeValues(processIndex2.seg))
			{
				_lastDeferredProcessProcessSlot = _nextDeferredProcessProcessSlot;
			}
			processIndex2.i = _nextDeferredProcessProcessSlot++;
			DeferredProcessProcesses[processIndex2.i] = coroutine;
			if (tag != null)
			{
				AddTag(tag, handle);
			}
			_indexToHandle.Add(processIndex2, handle);
			_handleToIndex.Add(handle, processIndex2);
			while (prewarm)
			{
				if (!DeferredProcessProcesses[processIndex2.i].MoveNext())
				{
					if (_indexToHandle.ContainsKey(processIndex2))
					{
						KillCoroutinesOnInstance(_indexToHandle[processIndex2]);
					}
					prewarm = false;
				}
				else if (DeferredProcessProcesses[processIndex2.i] != null && double.IsNaN(DeferredProcessProcesses[processIndex2.i].Current))
				{
					if (ReplacementFunction != null)
					{
						DeferredProcessProcesses[processIndex2.i] = ReplacementFunction(DeferredProcessProcesses[processIndex2.i], _indexToHandle[processIndex2]);
						ReplacementFunction = null;
					}
					prewarm = !DeferredProcessPaused[processIndex2.i] && !DeferredProcessHeld[processIndex2.i];
				}
				else
				{
					prewarm = false;
				}
			}
			break;
		default:
			handle = default(CoroutineHandle);
			break;
		}
		localTime = num;
		deltaTime = num2;
		currentCoroutine = coroutineHandle;
		return handle;
	}

	public static int KillCoroutines()
	{
		return (_instance != null) ? _instance.KillCoroutinesOnInstance() : 0;
	}

	public int KillCoroutinesOnInstance()
	{
		int result = _nextProcessProcessSlot + _nextDeferredProcessProcessSlot + _nextPhysicsProcessProcessSlot;
		ProcessProcesses = new IEnumerator<double>[256];
		ProcessPaused = new bool[256];
		ProcessHeld = new bool[256];
		ProcessCoroutines = 0;
		_nextProcessProcessSlot = 0;
		DeferredProcessProcesses = new IEnumerator<double>[8];
		DeferredProcessPaused = new bool[8];
		DeferredProcessHeld = new bool[8];
		DeferredProcessCoroutines = 0;
		_nextDeferredProcessProcessSlot = 0;
		PhysicsProcessProcesses = new IEnumerator<double>[64];
		PhysicsProcessPaused = new bool[64];
		PhysicsProcessHeld = new bool[64];
		PhysicsProcessCoroutines = 0;
		_nextPhysicsProcessProcessSlot = 0;
		_processTags.Clear();
		_taggedProcesses.Clear();
		_handleToIndex.Clear();
		_indexToHandle.Clear();
		_waitingTriggers.Clear();
		_expansions = (ushort)(_expansions / 2 + 1);
		return result;
	}

	public static int KillCoroutines(CoroutineHandle handle)
	{
		return (ActiveInstances[handle.Key] != null) ? GetInstance(handle.Key).KillCoroutinesOnInstance(handle) : 0;
	}

	public int KillCoroutinesOnInstance(CoroutineHandle handle)
	{
		bool flag = false;
		if (_handleToIndex.ContainsKey(handle))
		{
			if (_waitingTriggers.ContainsKey(handle))
			{
				CloseWaitingProcess(handle);
			}
			flag = CoindexExtract(_handleToIndex[handle]) != null;
			RemoveTag(handle);
		}
		return flag ? 1 : 0;
	}

	public static int KillCoroutines(string tag)
	{
		return (_instance != null) ? _instance.KillCoroutinesOnInstance(tag) : 0;
	}

	public int KillCoroutinesOnInstance(string tag)
	{
		if (tag == null)
		{
			return 0;
		}
		int num = 0;
		while (_taggedProcesses.ContainsKey(tag))
		{
			HashSet<CoroutineHandle>.Enumerator enumerator = _taggedProcesses[tag].GetEnumerator();
			enumerator.MoveNext();
			if (Nullify(_handleToIndex[enumerator.Current]))
			{
				if (_waitingTriggers.ContainsKey(enumerator.Current))
				{
					CloseWaitingProcess(enumerator.Current);
				}
				num++;
			}
			RemoveTag(enumerator.Current);
			if (_handleToIndex.ContainsKey(enumerator.Current))
			{
				_indexToHandle.Remove(_handleToIndex[enumerator.Current]);
				_handleToIndex.Remove(enumerator.Current);
			}
		}
		return num;
	}

	public static int PauseCoroutines()
	{
		return (_instance != null) ? _instance.PauseCoroutinesOnInstance() : 0;
	}

	public int PauseCoroutinesOnInstance()
	{
		int num = 0;
		for (int i = 0; i < _nextProcessProcessSlot; i++)
		{
			if (!ProcessPaused[i] && ProcessProcesses[i] != null)
			{
				num++;
				ProcessPaused[i] = true;
				if (ProcessProcesses[i].Current > GetSegmentTime(Segment.Process))
				{
					ProcessProcesses[i] = _InjectDelay(ProcessProcesses[i], ProcessProcesses[i].Current - GetSegmentTime(Segment.Process));
				}
			}
		}
		for (int i = 0; i < _nextDeferredProcessProcessSlot; i++)
		{
			if (!DeferredProcessPaused[i] && DeferredProcessProcesses[i] != null)
			{
				num++;
				DeferredProcessPaused[i] = true;
				if (DeferredProcessProcesses[i].Current > GetSegmentTime(Segment.DeferredProcess))
				{
					DeferredProcessProcesses[i] = _InjectDelay(DeferredProcessProcesses[i], DeferredProcessProcesses[i].Current - GetSegmentTime(Segment.DeferredProcess));
				}
			}
		}
		for (int i = 0; i < _nextPhysicsProcessProcessSlot; i++)
		{
			if (!PhysicsProcessPaused[i] && PhysicsProcessProcesses[i] != null)
			{
				num++;
				PhysicsProcessPaused[i] = true;
				if (PhysicsProcessProcesses[i].Current > GetSegmentTime(Segment.PhysicsProcess))
				{
					PhysicsProcessProcesses[i] = _InjectDelay(PhysicsProcessProcesses[i], PhysicsProcessProcesses[i].Current - GetSegmentTime(Segment.PhysicsProcess));
				}
			}
		}
		return num;
	}

	public static int PauseCoroutines(CoroutineHandle handle)
	{
		return (ActiveInstances[handle.Key] != null) ? GetInstance(handle.Key).PauseCoroutinesOnInstance(handle) : 0;
	}

	public int PauseCoroutinesOnInstance(CoroutineHandle handle)
	{
		return (_handleToIndex.ContainsKey(handle) && !CoindexIsNull(_handleToIndex[handle]) && !SetPause(_handleToIndex[handle], newPausedState: true)) ? 1 : 0;
	}

	public static int PauseCoroutines(string tag)
	{
		return (_instance != null) ? _instance.PauseCoroutinesOnInstance(tag) : 0;
	}

	public int PauseCoroutinesOnInstance(string tag)
	{
		if (tag == null || !_taggedProcesses.ContainsKey(tag))
		{
			return 0;
		}
		int num = 0;
		HashSet<CoroutineHandle>.Enumerator enumerator = _taggedProcesses[tag].GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!CoindexIsNull(_handleToIndex[enumerator.Current]) && !SetPause(_handleToIndex[enumerator.Current], newPausedState: true))
			{
				num++;
			}
		}
		return num;
	}

	public static int ResumeCoroutines()
	{
		return (_instance != null) ? _instance.ResumeCoroutinesOnInstance() : 0;
	}

	public int ResumeCoroutinesOnInstance()
	{
		int num = 0;
		ProcessIndex processIndex = default(ProcessIndex);
		processIndex.i = 0;
		processIndex.seg = Segment.Process;
		while (processIndex.i < _nextProcessProcessSlot)
		{
			if (ProcessPaused[processIndex.i] && ProcessProcesses[processIndex.i] != null)
			{
				ProcessPaused[processIndex.i] = false;
				num++;
			}
			processIndex.i++;
		}
		processIndex.i = 0;
		processIndex.seg = Segment.DeferredProcess;
		while (processIndex.i < _nextDeferredProcessProcessSlot)
		{
			if (DeferredProcessPaused[processIndex.i] && DeferredProcessProcesses[processIndex.i] != null)
			{
				DeferredProcessPaused[processIndex.i] = false;
				num++;
			}
			processIndex.i++;
		}
		processIndex.i = 0;
		processIndex.seg = Segment.PhysicsProcess;
		while (processIndex.i < _nextPhysicsProcessProcessSlot)
		{
			if (PhysicsProcessPaused[processIndex.i] && PhysicsProcessProcesses[processIndex.i] != null)
			{
				PhysicsProcessPaused[processIndex.i] = false;
				num++;
			}
			processIndex.i++;
		}
		return num;
	}

	public static int ResumeCoroutines(CoroutineHandle handle)
	{
		return (ActiveInstances[handle.Key] != null) ? GetInstance(handle.Key).ResumeCoroutinesOnInstance(handle) : 0;
	}

	public int ResumeCoroutinesOnInstance(CoroutineHandle handle)
	{
		return (_handleToIndex.ContainsKey(handle) && !CoindexIsNull(_handleToIndex[handle]) && SetPause(_handleToIndex[handle], newPausedState: false)) ? 1 : 0;
	}

	public static int ResumeCoroutines(string tag)
	{
		return (_instance != null) ? _instance.ResumeCoroutinesOnInstance(tag) : 0;
	}

	public int ResumeCoroutinesOnInstance(string tag)
	{
		if (tag == null || !_taggedProcesses.ContainsKey(tag))
		{
			return 0;
		}
		int num = 0;
		HashSet<CoroutineHandle>.Enumerator enumerator = _taggedProcesses[tag].GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!CoindexIsNull(_handleToIndex[enumerator.Current]) && SetPause(_handleToIndex[enumerator.Current], newPausedState: false))
			{
				num++;
			}
		}
		return num;
	}

	private bool UpdateTimeValues(Segment segment)
	{
		switch (segment)
		{
		case Segment.Process:
			if (_currentProcessFrame != Engine.GetProcessFrames())
			{
				deltaTime = GetProcessDeltaTime();
				_lastProcessTime += deltaTime;
				localTime = _lastProcessTime;
				_currentProcessFrame = Engine.GetProcessFrames();
				return true;
			}
			deltaTime = GetProcessDeltaTime();
			localTime = _lastProcessTime;
			return false;
		case Segment.DeferredProcess:
			if (_currentDeferredProcessFrame != Engine.GetProcessFrames())
			{
				deltaTime = GetProcessDeltaTime();
				_lastDeferredProcessTime += deltaTime;
				localTime = _lastDeferredProcessTime;
				_currentDeferredProcessFrame = Engine.GetProcessFrames();
				return true;
			}
			deltaTime = GetProcessDeltaTime();
			localTime = _lastDeferredProcessTime;
			return false;
		case Segment.PhysicsProcess:
			deltaTime = GetPhysicsProcessDeltaTime();
			_physicsProcessTime += deltaTime;
			localTime = _physicsProcessTime;
			if (_lastPhysicsProcessTime + 9.999999747378752E-05 < _physicsProcessTime)
			{
				_lastPhysicsProcessTime = _physicsProcessTime;
				return true;
			}
			return false;
		default:
			return true;
		}
	}

	private double GetSegmentTime(Segment segment)
	{
		switch (segment)
		{
		case Segment.Process:
			if (_currentProcessFrame == Engine.GetProcessFrames())
			{
				return _lastProcessTime;
			}
			return _lastProcessTime + GetProcessDeltaTime();
		case Segment.DeferredProcess:
			if (_currentProcessFrame == Engine.GetProcessFrames())
			{
				return _lastDeferredProcessTime;
			}
			return _lastDeferredProcessTime + GetProcessDeltaTime();
		case Segment.PhysicsProcess:
			return _physicsProcessTime;
		default:
			return 0.0;
		}
	}

	public static Timing GetInstance(byte ID)
	{
		if (ID >= 16)
		{
			return null;
		}
		return ActiveInstances[ID];
	}

	private void AddTag(string tag, CoroutineHandle coindex)
	{
		_processTags.Add(coindex, tag);
		if (_taggedProcesses.ContainsKey(tag))
		{
			_taggedProcesses[tag].Add(coindex);
			return;
		}
		_taggedProcesses.Add(tag, new HashSet<CoroutineHandle> { coindex });
	}

	private void RemoveTag(CoroutineHandle coindex)
	{
		if (_processTags.ContainsKey(coindex))
		{
			if (_taggedProcesses[_processTags[coindex]].Count > 1)
			{
				_taggedProcesses[_processTags[coindex]].Remove(coindex);
			}
			else
			{
				_taggedProcesses.Remove(_processTags[coindex]);
			}
			_processTags.Remove(coindex);
		}
	}

	private bool Nullify(ProcessIndex coindex)
	{
		switch (coindex.seg)
		{
		case Segment.Process:
		{
			bool result = ProcessProcesses[coindex.i] != null;
			ProcessProcesses[coindex.i] = null;
			return result;
		}
		case Segment.PhysicsProcess:
		{
			bool result = PhysicsProcessProcesses[coindex.i] != null;
			PhysicsProcessProcesses[coindex.i] = null;
			return result;
		}
		case Segment.DeferredProcess:
		{
			bool result = DeferredProcessProcesses[coindex.i] != null;
			DeferredProcessProcesses[coindex.i] = null;
			return result;
		}
		default:
			return false;
		}
	}

	private IEnumerator<double> CoindexExtract(ProcessIndex coindex)
	{
		switch (coindex.seg)
		{
		case Segment.Process:
		{
			IEnumerator<double> result = ProcessProcesses[coindex.i];
			ProcessProcesses[coindex.i] = null;
			return result;
		}
		case Segment.PhysicsProcess:
		{
			IEnumerator<double> result = PhysicsProcessProcesses[coindex.i];
			PhysicsProcessProcesses[coindex.i] = null;
			return result;
		}
		case Segment.DeferredProcess:
		{
			IEnumerator<double> result = DeferredProcessProcesses[coindex.i];
			DeferredProcessProcesses[coindex.i] = null;
			return result;
		}
		default:
			return null;
		}
	}

	private IEnumerator<double> CoindexPeek(ProcessIndex coindex)
	{
		return coindex.seg switch
		{
			Segment.Process => ProcessProcesses[coindex.i], 
			Segment.PhysicsProcess => PhysicsProcessProcesses[coindex.i], 
			Segment.DeferredProcess => DeferredProcessProcesses[coindex.i], 
			_ => null, 
		};
	}

	private bool CoindexIsNull(ProcessIndex coindex)
	{
		return coindex.seg switch
		{
			Segment.Process => ProcessProcesses[coindex.i] == null, 
			Segment.PhysicsProcess => PhysicsProcessProcesses[coindex.i] == null, 
			Segment.DeferredProcess => DeferredProcessProcesses[coindex.i] == null, 
			_ => true, 
		};
	}

	private bool SetPause(ProcessIndex coindex, bool newPausedState)
	{
		if (CoindexPeek(coindex) == null)
		{
			return false;
		}
		switch (coindex.seg)
		{
		case Segment.Process:
		{
			bool result = ProcessPaused[coindex.i];
			ProcessPaused[coindex.i] = newPausedState;
			if (newPausedState && ProcessProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
			{
				ProcessProcesses[coindex.i] = _InjectDelay(ProcessProcesses[coindex.i], ProcessProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));
			}
			return result;
		}
		case Segment.PhysicsProcess:
		{
			bool result = PhysicsProcessPaused[coindex.i];
			PhysicsProcessPaused[coindex.i] = newPausedState;
			if (newPausedState && PhysicsProcessProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
			{
				PhysicsProcessProcesses[coindex.i] = _InjectDelay(PhysicsProcessProcesses[coindex.i], PhysicsProcessProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));
			}
			return result;
		}
		case Segment.DeferredProcess:
		{
			bool result = DeferredProcessPaused[coindex.i];
			DeferredProcessPaused[coindex.i] = newPausedState;
			if (newPausedState && DeferredProcessProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
			{
				DeferredProcessProcesses[coindex.i] = _InjectDelay(DeferredProcessProcesses[coindex.i], DeferredProcessProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));
			}
			return result;
		}
		default:
			return false;
		}
	}

	private bool SetHeld(ProcessIndex coindex, bool newHeldState)
	{
		if (CoindexPeek(coindex) == null)
		{
			return false;
		}
		switch (coindex.seg)
		{
		case Segment.Process:
		{
			bool result = ProcessHeld[coindex.i];
			ProcessHeld[coindex.i] = newHeldState;
			if (newHeldState && ProcessProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
			{
				ProcessProcesses[coindex.i] = _InjectDelay(ProcessProcesses[coindex.i], ProcessProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));
			}
			return result;
		}
		case Segment.PhysicsProcess:
		{
			bool result = PhysicsProcessHeld[coindex.i];
			PhysicsProcessHeld[coindex.i] = newHeldState;
			if (newHeldState && PhysicsProcessProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
			{
				PhysicsProcessProcesses[coindex.i] = _InjectDelay(PhysicsProcessProcesses[coindex.i], PhysicsProcessProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));
			}
			return result;
		}
		case Segment.DeferredProcess:
		{
			bool result = DeferredProcessHeld[coindex.i];
			DeferredProcessHeld[coindex.i] = newHeldState;
			if (newHeldState && DeferredProcessProcesses[coindex.i].Current > GetSegmentTime(coindex.seg))
			{
				DeferredProcessProcesses[coindex.i] = _InjectDelay(DeferredProcessProcesses[coindex.i], DeferredProcessProcesses[coindex.i].Current - GetSegmentTime(coindex.seg));
			}
			return result;
		}
		default:
			return false;
		}
	}

	private IEnumerator<double> _InjectDelay(IEnumerator<double> proc, double delayTime)
	{
		yield return WaitForSecondsOnInstance(delayTime);
		_tmpRef = proc;
		ReplacementFunction = ReturnTmpRefForRepFunc;
		yield return double.NaN;
	}

	private bool CoindexIsPaused(ProcessIndex coindex)
	{
		return coindex.seg switch
		{
			Segment.Process => ProcessPaused[coindex.i], 
			Segment.PhysicsProcess => PhysicsProcessPaused[coindex.i], 
			Segment.DeferredProcess => DeferredProcessPaused[coindex.i], 
			_ => false, 
		};
	}

	private bool CoindexIsHeld(ProcessIndex coindex)
	{
		return coindex.seg switch
		{
			Segment.Process => ProcessHeld[coindex.i], 
			Segment.PhysicsProcess => PhysicsProcessHeld[coindex.i], 
			Segment.DeferredProcess => DeferredProcessHeld[coindex.i], 
			_ => false, 
		};
	}

	private void CoindexReplace(ProcessIndex coindex, IEnumerator<double> replacement)
	{
		switch (coindex.seg)
		{
		case Segment.Process:
			ProcessProcesses[coindex.i] = replacement;
			break;
		case Segment.PhysicsProcess:
			PhysicsProcessProcesses[coindex.i] = replacement;
			break;
		case Segment.DeferredProcess:
			DeferredProcessProcesses[coindex.i] = replacement;
			break;
		}
	}

	public static double WaitForSeconds(double waitTime)
	{
		if (double.IsNaN(waitTime))
		{
			waitTime = 0.0;
		}
		return Instance.localTime + waitTime;
	}

	public double WaitForSecondsOnInstance(double waitTime)
	{
		if (double.IsNaN(waitTime))
		{
			waitTime = 0.0;
		}
		return localTime + waitTime;
	}

	public static double WaitUntilDone(CoroutineHandle otherCoroutine)
	{
		return WaitUntilDone(otherCoroutine, warnOnIssue: true);
	}

	public static double WaitUntilDone(CoroutineHandle otherCoroutine, bool warnOnIssue)
	{
		Timing instance = GetInstance(otherCoroutine.Key);
		if (instance != null && instance._handleToIndex.ContainsKey(otherCoroutine))
		{
			if (instance.CoindexIsNull(instance._handleToIndex[otherCoroutine]))
			{
				return 0.0;
			}
			if (!instance._waitingTriggers.ContainsKey(otherCoroutine))
			{
				instance.CoindexReplace(instance._handleToIndex[otherCoroutine], instance._StartWhenDone(otherCoroutine, instance.CoindexPeek(instance._handleToIndex[otherCoroutine])));
				instance._waitingTriggers.Add(otherCoroutine, new HashSet<CoroutineHandle>());
			}
			if (instance.currentCoroutine == otherCoroutine)
			{
				if (warnOnIssue)
				{
					GD.PrintErr("A coroutine cannot wait for itself.");
				}
				return double.NegativeInfinity;
			}
			if (!instance.currentCoroutine.IsValid)
			{
				if (warnOnIssue)
				{
					GD.PrintErr("The two coroutines are not running on the same MEC instance.");
				}
				return double.NegativeInfinity;
			}
			instance._waitingTriggers[otherCoroutine].Add(instance.currentCoroutine);
			if (!instance._allWaiting.Contains(instance.currentCoroutine))
			{
				instance._allWaiting.Add(instance.currentCoroutine);
			}
			instance.SetHeld(instance._handleToIndex[instance.currentCoroutine], newHeldState: true);
			instance.SwapToLast(otherCoroutine, instance.currentCoroutine);
			return double.NaN;
		}
		if (warnOnIssue)
		{
			GD.PrintErr("WaitUntilDone cannot hold: The coroutine handle that was passed in is invalid.\n" + otherCoroutine);
		}
		return double.NegativeInfinity;
	}

	private IEnumerator<double> _StartWhenDone(CoroutineHandle handle, IEnumerator<double> proc)
	{
		if (!_waitingTriggers.ContainsKey(handle))
		{
			yield break;
		}
		try
		{
			if (proc.Current > localTime)
			{
				yield return proc.Current;
			}
			while (proc.MoveNext())
			{
				yield return proc.Current;
			}
		}
		finally
		{
			CloseWaitingProcess(handle);
		}
	}

	private void SwapToLast(CoroutineHandle firstHandle, CoroutineHandle lastHandle)
	{
		if (firstHandle.Key != lastHandle.Key)
		{
			return;
		}
		ProcessIndex processIndex = _handleToIndex[firstHandle];
		ProcessIndex processIndex2 = _handleToIndex[lastHandle];
		if (processIndex.seg != processIndex2.seg || processIndex.i < processIndex2.i)
		{
			return;
		}
		IEnumerator<double> replacement = CoindexPeek(processIndex);
		CoindexReplace(processIndex, CoindexPeek(processIndex2));
		CoindexReplace(processIndex2, replacement);
		_indexToHandle[processIndex] = lastHandle;
		_indexToHandle[processIndex2] = firstHandle;
		_handleToIndex[firstHandle] = processIndex2;
		_handleToIndex[lastHandle] = processIndex;
		bool newPausedState = SetPause(processIndex, CoindexIsPaused(processIndex2));
		SetPause(processIndex2, newPausedState);
		newPausedState = SetHeld(processIndex, CoindexIsHeld(processIndex2));
		SetHeld(processIndex2, newPausedState);
		if (_waitingTriggers.ContainsKey(lastHandle))
		{
			HashSet<CoroutineHandle>.Enumerator enumerator = _waitingTriggers[lastHandle].GetEnumerator();
			while (enumerator.MoveNext())
			{
				SwapToLast(lastHandle, enumerator.Current);
			}
		}
		if (!_allWaiting.Contains(firstHandle))
		{
			return;
		}
		Dictionary<CoroutineHandle, HashSet<CoroutineHandle>>.Enumerator enumerator2 = _waitingTriggers.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			HashSet<CoroutineHandle>.Enumerator enumerator3 = enumerator2.Current.Value.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				if (enumerator3.Current == firstHandle)
				{
					SwapToLast(enumerator2.Current.Key, firstHandle);
				}
			}
		}
	}

	private void CloseWaitingProcess(CoroutineHandle handle)
	{
		if (!_waitingTriggers.ContainsKey(handle))
		{
			return;
		}
		HashSet<CoroutineHandle>.Enumerator enumerator = _waitingTriggers[handle].GetEnumerator();
		_waitingTriggers.Remove(handle);
		while (enumerator.MoveNext())
		{
			if (_handleToIndex.ContainsKey(enumerator.Current) && !HandleIsInWaitingList(enumerator.Current))
			{
				SetHeld(_handleToIndex[enumerator.Current], newHeldState: false);
				_allWaiting.Remove(enumerator.Current);
			}
		}
	}

	private bool HandleIsInWaitingList(CoroutineHandle handle)
	{
		Dictionary<CoroutineHandle, HashSet<CoroutineHandle>>.Enumerator enumerator = _waitingTriggers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value.Contains(handle))
			{
				return true;
			}
		}
		return false;
	}

	private static IEnumerator<double> ReturnTmpRefForRepFunc(IEnumerator<double> coptr, CoroutineHandle handle)
	{
		return _tmpRef as IEnumerator<double>;
	}

	public bool LockCoroutine(CoroutineHandle coroutine, CoroutineHandle key)
	{
		if (coroutine.Key != _instanceID || key == default(CoroutineHandle) || key.Key != 0)
		{
			return false;
		}
		if (!_waitingTriggers.ContainsKey(key))
		{
			_waitingTriggers.Add(key, new HashSet<CoroutineHandle> { coroutine });
		}
		else
		{
			_waitingTriggers[key].Add(coroutine);
		}
		_allWaiting.Add(coroutine);
		SetHeld(_handleToIndex[coroutine], newHeldState: true);
		return true;
	}

	public bool UnlockCoroutine(CoroutineHandle coroutine, CoroutineHandle key)
	{
		if (coroutine.Key != _instanceID || key == default(CoroutineHandle) || !_handleToIndex.ContainsKey(coroutine) || !_waitingTriggers.ContainsKey(key))
		{
			return false;
		}
		if (_waitingTriggers[key].Count == 1)
		{
			_waitingTriggers.Remove(key);
		}
		else
		{
			_waitingTriggers[key].Remove(coroutine);
		}
		if (!HandleIsInWaitingList(coroutine))
		{
			SetHeld(_handleToIndex[coroutine], newHeldState: false);
			_allWaiting.Remove(coroutine);
		}
		return true;
	}

	public static CoroutineHandle CallDeferred(Action action)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._DelayedCall(0.0, action, null), Segment.DeferredProcess);
	}

	public CoroutineHandle CallDeferredOnInstance(Action action)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(_DelayedCall(0.0, action, null), Segment.DeferredProcess);
	}

	public static CoroutineHandle CallDelayed(double delay, Action action)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._DelayedCall(delay, action, null));
	}

	public CoroutineHandle CallDelayedOnInstance(double delay, Action action)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_DelayedCall(delay, action, null));
	}

	public static CoroutineHandle CallDelayed(double delay, Action action, Node cancelWith)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._DelayedCall(delay, action, cancelWith));
	}

	public CoroutineHandle CallDelayedOnInstance(double delay, Action action, Node cancelWith)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_DelayedCall(delay, action, cancelWith));
	}

	public static CoroutineHandle CallDelayed(double delay, Segment segment, Action action)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._DelayedCall(delay, action, null), segment);
	}

	public CoroutineHandle CallDelayedOnInstance(double delay, Segment segment, Action action)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_DelayedCall(delay, action, null), segment);
	}

	public static CoroutineHandle CallDelayed(double delay, Segment segment, Action action, Node node)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._DelayedCall(delay, action, node), segment);
	}

	public CoroutineHandle CallDelayedOnInstance(double delay, Segment segment, Action action, Node node)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_DelayedCall(delay, action, node), segment);
	}

	private IEnumerator<double> _DelayedCall(double delay, Action action, Node cancelWith)
	{
		yield return WaitForSecondsOnInstance(delay);
		if (cancelWith == null || cancelWith != null)
		{
			action();
		}
	}

	public static CoroutineHandle CallPeriodically(double timeframe, double period, Action action, Action onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._CallContinuously(timeframe, period, action, onDone), Segment.Process);
	}

	public CoroutineHandle CallPeriodicallyOnInstance(double timeframe, double period, Action action, Action onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_CallContinuously(timeframe, period, action, onDone), Segment.Process);
	}

	public static CoroutineHandle CallPeriodically(double timeframe, double period, Action action, Segment segment, Action onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._CallContinuously(timeframe, period, action, onDone), segment);
	}

	public CoroutineHandle CallPeriodicallyOnInstance(double timeframe, double period, Action action, Segment segment, Action onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_CallContinuously(timeframe, period, action, onDone), segment);
	}

	public static CoroutineHandle CallContinuously(double timeframe, Action action, Action onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._CallContinuously(timeframe, 0.0, action, onDone), Segment.Process);
	}

	public CoroutineHandle CallContinuouslyOnInstance(double timeframe, Action action, Action onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_CallContinuously(timeframe, 0.0, action, onDone), Segment.Process);
	}

	public static CoroutineHandle CallContinuously(double timeframe, Action action, Segment timing, Action onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._CallContinuously(timeframe, 0.0, action, onDone), timing);
	}

	public CoroutineHandle CallContinuouslyOnInstance(double timeframe, Action action, Segment timing, Action onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_CallContinuously(timeframe, 0.0, action, onDone), timing);
	}

	private IEnumerator<double> _CallContinuously(double timeframe, double period, Action action, Action onDone)
	{
		double startTime = localTime;
		while (localTime <= startTime + timeframe)
		{
			yield return WaitForSecondsOnInstance(period);
			action();
		}
		onDone?.Invoke();
	}

	public static CoroutineHandle CallPeriodically<T>(T reference, double timeframe, double period, Action<T> action, Action<T> onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._CallContinuously(reference, timeframe, period, action, onDone), Segment.Process);
	}

	public CoroutineHandle CallPeriodicallyOnInstance<T>(T reference, double timeframe, double period, Action<T> action, Action<T> onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_CallContinuously(reference, timeframe, period, action, onDone), Segment.Process);
	}

	public static CoroutineHandle CallPeriodically<T>(T reference, double timeframe, double period, Action<T> action, Segment timing, Action<T> onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._CallContinuously(reference, timeframe, period, action, onDone), timing);
	}

	public CoroutineHandle CallPeriodicallyOnInstance<T>(T reference, double timeframe, double period, Action<T> action, Segment timing, Action<T> onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_CallContinuously(reference, timeframe, period, action, onDone), timing);
	}

	public static CoroutineHandle CallContinuously<T>(T reference, double timeframe, Action<T> action, Action<T> onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._CallContinuously(reference, timeframe, 0.0, action, onDone), Segment.Process);
	}

	public CoroutineHandle CallContinuouslyOnInstance<T>(T reference, double timeframe, Action<T> action, Action<T> onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_CallContinuously(reference, timeframe, 0.0, action, onDone), Segment.Process);
	}

	public static CoroutineHandle CallContinuously<T>(T reference, double timeframe, Action<T> action, Segment timing, Action<T> onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutine(Instance._CallContinuously(reference, timeframe, 0.0, action, onDone), timing);
	}

	public CoroutineHandle CallContinuouslyOnInstance<T>(T reference, double timeframe, Action<T> action, Segment timing, Action<T> onDone = null)
	{
		return (action == null) ? default(CoroutineHandle) : RunCoroutineOnInstance(_CallContinuously(reference, timeframe, 0.0, action, onDone), timing);
	}

	private IEnumerator<double> _CallContinuously<T>(T reference, double timeframe, double period, Action<T> action, Action<T> onDone = null)
	{
		double startTime = localTime;
		while (localTime <= startTime + timeframe)
		{
			yield return WaitForSecondsOnInstance(period);
			action(reference);
		}
		onDone?.Invoke(reference);
	}
}
