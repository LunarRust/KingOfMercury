using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/EmeraldKeyBehavior.cs")]
public partial class EmeraldKeyBehavior : Node
{
	private GDScript Progress = (GDScript)GD.Load("res://Scripts/game_progress.gd");

	private GDScript ACH = (GDScript)GD.Load("res://Scripts/SteamAchievement.gd");

	[Export(PropertyHint.None, "")]
	public Sprite3D sigil;

	[Export(PropertyHint.None, "")]
	public Node3D model;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	[Export(PropertyHint.None, "")]
	public OmniLight3D light;

	private bool used;

	public void Touch()
	{
		if (!used)
		{
			used = true;
			getKey();
		}
	}

	public async void getKey()
	{
		soundSource.Play();
		Tween tween = CreateTween();
		tween.TweenProperty(sigil, "scale", Vector3.One, 2.0).SetTrans(Tween.TransitionType.Sine);
		await Task.Delay(TimeSpan.FromSeconds(3.0));
		Tween newtween = CreateTween();
		newtween.TweenProperty(model, "scale", Vector3.Zero, 2.0).SetTrans(Tween.TransitionType.Sine);
		Tween lightTween = CreateTween();
		lightTween.TweenProperty(light, "light_energy", 0, 2.0).SetTrans(Tween.TransitionType.Sine);
		await Task.Delay(TimeSpan.FromSeconds(2.0999999046325684));
		Variant currentKeys = Progress.Get("EmeraldKeys");
		Progress.Set("EmeraldKeys", (int)currentKeys + 1);
		GD.Print("You currently have " + Progress.Get("EmeraldKeys").ToString() + " Emerald Keys");
		ACH.Call("_static_grant", "ACH_KEY");
		GetParent().QueueFree();
	}
}
