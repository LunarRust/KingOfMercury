using Godot;

[ScriptPath("res://Scripts/CompassArrow.cs")]
public partial class CompassArrow : Sprite2D
{
	[Export(PropertyHint.None, "")]
	public Node3D playerNode;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		base.Rotation = playerNode.GlobalRotation.Y;
	}
}
