using Godot;

[ScriptPath("res://Scenes/ChainLever.cs")]
public partial class ChainLever : Node
{
	[Export(PropertyHint.None, "")]
	public Node3D chainModel;

	[Export(PropertyHint.None, "")]
	public Node3D Guillotine;

	[Export(PropertyHint.None, "")]
	public Node3D Queen1;

	[Export(PropertyHint.None, "")]
	public Node3D Queen2;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public Node3D EarthEgg;

	private AudioStream killSound;

	public bool used;

	public override void _Ready()
	{
		killSound = GD.Load("res://Sounds/QueenKill.ogg") as AudioStream;
		EarthEgg.GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
	}

	public void Touch()
	{
		if (!used)
		{
			AudioStreamPlayer node = GetNode<AudioStreamPlayer>("%SoundSource");
			node.Stream = killSound;
			node.Play();
			CameraShake.Shake(0.1f);
			anim.Play("TurnOn");
			Tween tween = CreateTween();
			tween.TweenProperty(chainModel, "position", Vector3.Up * 12f, 2.0).SetTrans(Tween.TransitionType.Sine).AsRelative();
			used = true;
			Guillotine.Position = new Vector3(9.538f, -2.859f, 63.533f);
			Queen1.QueueFree();
			Queen2.Show();
			EarthEgg.Show();
			EarthEgg.GetNode<CollisionShape3D>("CollisionShape3D").Disabled = false;
		}
	}
}
