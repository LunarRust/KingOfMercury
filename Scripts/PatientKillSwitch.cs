using Godot;

[ScriptPath("res://Scripts/PatientKillSwitch.cs")]
public partial class PatientKillSwitch : Node
{
	[Export(PropertyHint.None, "")]
	public Node Blob;

	[Export(PropertyHint.None, "")]
	public Node Head;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public Node3D EggObject;

	[Export(PropertyHint.None, "")]
	public Sprite3D[] TVs;

	[Export(PropertyHint.None, "")]
	public Node3D[] newTvs;

	[Export(PropertyHint.None, "")]
	public Node3D[] breathers;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D tvSound;

	public static int patKilled;

	private bool used;

	private Material blackMaterial;

	public override void _Ready()
	{
		patKilled = 0;
		blackMaterial = GD.Load("res://textures/Black.tres") as Material;
		EggObject = GetNode<Node3D>("%EggofEarth");
		GD.Print(EggObject.Name);
		EggObject.Hide();
		EggObject.GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
	}

	public void Touch()
	{
		if (!used)
		{
			Sprite3D[] tVs = TVs;
			foreach (Sprite3D sprite3D in tVs)
			{
				sprite3D.Modulate = Color.FromHsv(0f, 0f, 0f);
			}
			Node3D[] array = newTvs;
			foreach (Node3D node3D in array)
			{
				node3D.GetNode<AnimationPlayer>("AnimationPlayer").Stop();
				node3D.GetNode<MeshInstance3D>("Armature/Skeleton3D/Cube").SetSurfaceOverrideMaterial(1, blackMaterial);
			}
			Node3D[] array2 = breathers;
			foreach (Node3D node3D2 in array2)
			{
				node3D2.GetNode<AnimationPlayer>("AnimationPlayer").Stop();
			}
			tvSound.Stop();
			AudioStreamPlayer node = GetNode<AudioStreamPlayer>("%SoundSource");
			node.Stream = GD.Load("res://Sounds/PatientKill.ogg") as AudioStream;
			node.Play();
			CameraShake.Shake(0.1f);
			anim.Play("TurnOn");
			used = true;
			Blob.GetNode<AnimationPlayer>("AnimationPlayer").Stop();
			Head.GetNode<AnimationPlayer>("PatientHead/AnimationPlayer").Play("Dead");
			Head.GetNode<MeshInstance3D>("PatientHead/Armature/Skeleton3D/TV").SetSurfaceOverrideMaterial(1, blackMaterial);
			Head.GetNode<DialogueSystem>("DialogueSystem").Dialogue = "...";
			Head.GetNode<DialogueSystem>("DialogueSystem").TouchDescription = "Slimey and Cold.";
			Head.GetNode<DialogueSystem>("DialogueSystem").LookDescription = "The figure is dead, and is slumped over in its metal cast.";
			patKilled++;
			if (patKilled > 2 && EggObject != null)
			{
				EggObject.Show();
				EggObject.GetNode<CollisionShape3D>("CollisionShape3D").Disabled = false;
			}
		}
	}
}
