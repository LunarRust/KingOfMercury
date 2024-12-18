using Godot;

[ScriptPath("res://Scripts/AnimationRandomizer.cs")]
public partial class AnimationRandomizer : Node
{
	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public float range = 3f;

	public override void _Ready()
	{
		anim.Stop();
		RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
		anim.Seek(randomNumberGenerator.RandfRange(0f, range), update: true);
		anim.Play();
	}
}
