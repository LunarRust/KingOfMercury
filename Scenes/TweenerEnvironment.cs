using Godot;

[ScriptPath("res://Scenes/TweenerEnvironment.cs")]
public partial class TweenerEnvironment : WorldEnvironment
{
	[Export(PropertyHint.None, "")]
	public string targetProperty;

	[Export(PropertyHint.None, "")]
	public float endValue;

	[Export(PropertyHint.None, "")]
	public float time = 1f;

	[Export(PropertyHint.None, "")]
	public Tween.TransitionType Transition = Tween.TransitionType.Quad;

	public override void _Ready()
	{
		float num = (float)base.Environment.Get(targetProperty);
		Tween tween = CreateTween().SetLoops();
		tween.TweenProperty(base.Environment, targetProperty, endValue, time).SetTrans(Transition).AsRelative();
		tween.TweenProperty(base.Environment, targetProperty, num, time).SetTrans(Transition);
	}
}
