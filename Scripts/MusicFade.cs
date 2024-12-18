using Godot;

[ScriptPath("res://Scripts/MusicFade.cs")]
public partial class MusicFade : Area3D
{
	[Export(PropertyHint.None, "")]
	public bool FadeIn;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer musicSource;

	private float initValue;

	public override void _Ready()
	{
		initValue = musicSource.VolumeDb;
	}

	private void OnAreaEntered(Node3D playerNode)
	{
		if (FadeIn)
		{
			Tween tween = CreateTween();
			tween.TweenProperty(musicSource, "volume_db", initValue, 5.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		}
		else
		{
			Tween tween2 = CreateTween();
			tween2.TweenProperty(musicSource, "volume_db", -80, 5.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		}
	}
}
