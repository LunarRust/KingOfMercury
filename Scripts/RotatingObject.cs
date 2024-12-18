using Godot;

[ScriptPath("res://Scripts/RotatingObject.cs")]
public partial class RotatingObject : Node3D
{
	[Export(PropertyHint.None, "")]
	public Vector3 rotationVector = Vector3.Up;

	[Export(PropertyHint.None, "")]
	public float speed = 1f;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		Rotate(rotationVector.Normalized(), speed * (float)delta);
	}
}
