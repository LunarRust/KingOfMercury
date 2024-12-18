using Godot;

[ScriptPath("res://Scripts/TweenerProperty.cs")]
public partial class TweenerProperty : Node
{
	[Export(PropertyHint.None, "")]
	public Node targetNode;

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
		float num = (float)targetNode.Get(targetProperty);
		Tween tween = CreateTween().SetLoops();
		tween.TweenProperty(targetNode, targetProperty, endValue, time).SetTrans(Transition).AsRelative();
		tween.TweenProperty(targetNode, targetProperty, num, time).SetTrans(Transition);
	}
}
