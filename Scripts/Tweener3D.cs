using Godot;

[ScriptPath("res://Scripts/Tweener3D.cs")]
public partial class Tweener3D : Node3D
{
	[Export(PropertyHint.None, "")]
	public float Time;

	[Export(PropertyHint.None, "")]
	public Tween.TransitionType Transition = Tween.TransitionType.Quad;

	[Export(PropertyHint.None, "")]
	public bool tweenPosition;

	[Export(PropertyHint.None, "")]
	public Vector3 endPosition;

	[Export(PropertyHint.None, "")]
	public bool tweenRotation;

	[Export(PropertyHint.None, "")]
	public Vector3 endrotation;

	[Export(PropertyHint.None, "")]
	public bool tweenScale;

	[Export(PropertyHint.None, "")]
	public float endScale;

	private Vector3 startPos;

	private Vector3 startRot;

	private float startScale;

	public override void _Ready()
	{
		startPos = base.Position;
		startRot = base.Rotation;
		startScale = base.Scale.X;
		GD.Print(startPos);
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
			tween.TweenProperty(this, "position", endPosition, Time).SetTrans(Transition).AsRelative();
			tween.TweenProperty(this, "position", startPos, Time).SetTrans(Transition);
		}
		if (tweenRotation)
		{
			Tween tween2 = CreateTween().SetLoops();
			tween2.TweenProperty(this, "rotation", endrotation, Time).SetTrans(Transition).AsRelative();
			tween2.TweenProperty(this, "rotation", startRot, Time).SetTrans(Transition);
		}
		if (tweenScale)
		{
			Tween tween3 = CreateTween().SetLoops();
			tween3.TweenProperty(this, "scale", Vector3.One * endScale, Time).SetTrans(Transition).AsRelative();
			tween3.TweenProperty(this, "scale", Vector3.One * startScale, Time).SetTrans(Transition);
		}
	}
}
