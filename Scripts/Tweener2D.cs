using Godot;

[ScriptPath("res://Scripts/Tweener2D.cs")]
public partial class Tweener2D : Node2D
{
	[Export(PropertyHint.None, "")]
	public float Time;

	[Export(PropertyHint.None, "")]
	public bool tweenPosition;

	[Export(PropertyHint.None, "")]
	public Vector2 endPosition;

	[Export(PropertyHint.None, "")]
	public bool tweenRotation;

	[Export(PropertyHint.None, "")]
	public float endrotation;

	[Export(PropertyHint.None, "")]
	public bool tweenScale;

	[Export(PropertyHint.None, "")]
	public float endScale;

	private Vector2 startPos;

	private float startRot;

	private float startScale;

	public override void _Ready()
	{
		startPos = base.Position;
		startRot = base.Rotation;
		startScale = base.Scale.X;
		startTween();
	}

	public override void _Process(double delta)
	{
	}

	public void startTween()
	{
		if (tweenPosition)
		{
			Tween tween = CreateTween().SetLoops();
			tween.TweenProperty(this, "position", endPosition, Time).SetTrans(Tween.TransitionType.Quad).AsRelative();
			tween.TweenProperty(this, "position", startPos, Time).SetTrans(Tween.TransitionType.Quad);
		}
		if (tweenRotation)
		{
			Tween tween2 = CreateTween().SetLoops();
			tween2.TweenProperty(this, "rotation", endrotation, Time).SetTrans(Tween.TransitionType.Quad).AsRelative();
			tween2.TweenProperty(this, "rotation", startRot, Time).SetTrans(Tween.TransitionType.Quad);
		}
		if (tweenScale)
		{
			Tween tween3 = CreateTween().SetLoops();
			tween3.TweenProperty(this, "scale", Vector2.One * endScale, Time).SetTrans(Tween.TransitionType.Quad).AsRelative();
			tween3.TweenProperty(this, "scale", Vector2.One * startScale, Time).SetTrans(Tween.TransitionType.Quad);
		}
	}
}
