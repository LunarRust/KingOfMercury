using Godot;

[ScriptPath("res://RandomSizeTween.cs")]
public partial class RandomSizeTween : Node2D
{
	[Export(PropertyHint.None, "")]
	public float minSize;

	[Export(PropertyHint.None, "")]
	public float maxSize;

	[Export(PropertyHint.None, "")]
	public float tweenTime;

	private RandomNumberGenerator rng;

	private Tween tween;

	public override void _Ready()
	{
		rng = new RandomNumberGenerator();
		startTween();
	}

	public override void _Process(double delta)
	{
	}

	public void startTween()
	{
		Tween tween = CreateTween().SetLoops();
		tween.TweenProperty(this, "scale", Vector2.One * maxSize, tweenTime).SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(this, "scale", Vector2.One * minSize, tweenTime).SetTrans(Tween.TransitionType.Quad);
	}
}
