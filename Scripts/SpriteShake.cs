using Godot;

[ScriptPath("res://Scripts/SpriteShake.cs")]
public partial class SpriteShake : Node2D
{
	private Vector2 startPos;

	[Export(PropertyHint.None, "")]
	public float power = 1f;

	private RandomNumberGenerator rng;

	public override void _Ready()
	{
		startPos = base.Position;
		rng = new RandomNumberGenerator();
	}

	public override void _Process(double delta)
	{
		base.Position = new Vector2(startPos.X + rng.RandfRange(-1f, 1f) * power, startPos.Y + rng.RandfRange(-1f, 1f) * power);
	}
}
