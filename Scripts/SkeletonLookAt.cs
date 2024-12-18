using Godot;

[ScriptPath("res://Scripts/SkeletonLookAt.cs")]
public partial class SkeletonLookAt : Skeleton3D
{
	[Export(PropertyHint.None, "")]
	public string boneName = "Head";

	private int boneIndex;

	[Export(PropertyHint.None, "")]
	public Node3D target;

	public override void _Ready()
	{
		boneIndex = FindBone(boneName);
	}

	public override void _Process(double delta)
	{
	}
}
