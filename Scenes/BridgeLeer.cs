using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scenes/BridgeLeer.cs")]
public partial class BridgeLeer : Node
{
	[Export(PropertyHint.None, "")]
	public Node3D BridgeObject;

	[Export(PropertyHint.None, "")]
	public Camera3D bridgeCam;

	private Camera3D playerCam;

	[Export(PropertyHint.None, "")]
	public Node3D NPC1;

	[Export(PropertyHint.None, "")]
	public Node3D NPC2;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer BridgeAnim1;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer BridgeAnim2;

	public bool used;

	public override void _Ready()
	{
		playerCam = CameraCast.instance;
		BridgeObject.GetNode<CollisionShape3D>("BridgeStatic/CollisionShape3D").Disabled = true;
		BridgeObject.Hide();
	}

	public async void Touch()
	{
		if (!used)
		{
			AudioStreamPlayer soundSource = GetNode<AudioStreamPlayer>("%SoundSource");
			BridgeObject.GetNode<CollisionShape3D>("BridgeStatic/CollisionShape3D").Disabled = false;
			soundSource.Stream = GD.Load("res://Sounds/Lever.ogg") as AudioStream;
			soundSource.Play();
			used = true;
			CameraShake.Shake(0.1f);
			anim.Play("TurnOn");
			await Task.Delay(TimeSpan.FromSeconds(1.0));
			Vector3 bridgePos = BridgeObject.Position;
			BridgeObject.Position -= Vector3.Up * 8f;
			hudmanager.HideHUD();
			bridgeCam.MakeCurrent();
			Tween tween = CreateTween();
			tween.TweenProperty(BridgeObject, "position", bridgePos, 5.0).SetTrans(Tween.TransitionType.Sine);
			soundSource.Stream = GD.Load("res://Sounds/ElevatorSlide.ogg") as AudioStream;
			soundSource.Play();
			BridgeAnim1.Play("Lift");
			BridgeAnim2.Play("Lift");
			await Task.Delay(TimeSpan.FromSeconds(6.0));
			playerCam.MakeCurrent();
			hudmanager.ShowHUD();
			if (GodotObject.IsInstanceValid(NPC1))
			{
				NPC1.Hide();
				NPC1.ProcessMode = ProcessModeEnum.Disabled;
				NPC2.Show();
			}
			else
			{
				NPC2.QueueFree();
			}
		}
	}
}
