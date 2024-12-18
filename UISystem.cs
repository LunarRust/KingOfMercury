using Godot;

[ScriptPath("res://UISystem.cs")]
public partial class UISystem : Sprite2D
{
	[Export(PropertyHint.None, "")]
	public Sprite2D faceSprite;

	public override void _Ready()
	{
		Tween tween = CreateTween().SetLoops();
		tween.TweenProperty(faceSprite, "rotation", 0.2, 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(faceSprite, "rotation", -0.2, 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
	}
}
