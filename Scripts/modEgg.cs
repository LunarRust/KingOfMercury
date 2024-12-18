using Godot;

[ScriptPath("res://Scripts/modEgg.cs")]
public partial class modEgg : Node
{
	private GDScript Fader = (GDScript)GD.Load("res://addons/UniversalFade/Fade.gd");

	public void Touch()
	{
		Fader.CallDeferred("crossfade_prepare", 3, "WeirdWipe", false, false);
		GetTree().CallDeferred("change_scene_to_file", "res://Scenes/modMenu.tscn");
		Fader.CallDeferred("crossfade_execute");
	}
}
