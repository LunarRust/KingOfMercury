using Godot;

[ScriptPath("res://Scripts/Sprite3DShake.cs")]
public partial class Sprite3DShake : Node3D
{
	private Vector3 startPos;

	[Export(PropertyHint.None, "")]
	public float power = 0.05f;

	private RandomNumberGenerator rng;

	public override void _Ready()
	{
		startPos = base.Position;
		rng = new RandomNumberGenerator();
	}

	public override void _Process(double delta)
	{
		base.Position = new Vector3(startPos.X + rng.RandfRange(-1f, 1f) * power, startPos.Y + rng.RandfRange(-1f, 1f) * power, startPos.Z + rng.RandfRange(-1f, 1f) * power);
	}
}
