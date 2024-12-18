using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/DNAGateLeverBehavior.cs")]
public partial class DNAGateLeverBehavior : Node
{
	[Export(PropertyHint.None, "")]
	public StaticBody3D gate1;

	[Export(PropertyHint.None, "")]
	public StaticBody3D gate2;

	[Export(PropertyHint.None, "")]
	private AnimationPlayer leverAnim;

	[Export(PropertyHint.None, "")]
	private AnimationPlayer GateAnim1;

	[Export(PropertyHint.None, "")]
	private AnimationPlayer GateAnim2;

	[Export(PropertyHint.None, "")]
	public Camera3D gateCam;

	[Export(PropertyHint.None, "")]
	public Camera3D playercam;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer soundSource;

	private Node3D playerObject;

	private bool used = false;

	public override void _Ready()
	{
		playercam = GetViewport().GetCamera3D();
		playerObject = GetTree().GetFirstNodeInGroup("player") as Node3D;
	}

	public void Touch()
	{
		if (!used)
		{
			OpenGate();
		}
	}

	public async void OpenGate()
	{
		used = true;
		soundSource.Play();
		playerObject.ProcessMode = ProcessModeEnum.Disabled;
		leverAnim.Play("TurnOn");
		await Task.Delay(TimeSpan.FromSeconds(1.0));
		hudmanager.HideHUD();
		gateCam.MakeCurrent();
		await Task.Delay(TimeSpan.FromSeconds(0.5));
		GateAnim1.Play("Open");
		GateAnim2.Play("Open");
		gate1.QueueFree();
		gate2.QueueFree();
		await Task.Delay(TimeSpan.FromSeconds(3.0));
		playercam.MakeCurrent();
		hudmanager.ShowHUD();
		playerObject.ProcessMode = ProcessModeEnum.Inherit;
	}
}
