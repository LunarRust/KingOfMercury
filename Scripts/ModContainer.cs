using Godot;

[ScriptPath("res://Scripts/ModContainer.cs")]
public partial class ModContainer : ColorRect
{
	public string location;

	public string WorkshopId;

	[Export(PropertyHint.None, "")]
	public Button playButton;

	[Export(PropertyHint.None, "")]
	public Button WorkshopButton;

	private GDScript Fader = (GDScript)GD.Load("res://addons/UniversalFade/Fade.gd");

	public string initMap = "res://modmaps/INIT.tscn";

	public override void _Ready()
	{
		playButton.Pressed += startMap;
	}

	public void startMap()
	{
		GD.Print("Attempting to load resource pack from " + location);
		if (ProjectSettings.LoadResourcePack(location))
		{
			GD.Print("Mod Loading was successful!");
			GD.Print("Attempting to change map to " + initMap);
			if (ResourceLoader.Exists(initMap))
			{
				GD.Print("Found INIT map. Starting Map Pack");
				Fader.CallDeferred("crossfade_prepare", 3, "WeirdWipe", false, false);
				GetTree().CallDeferred("change_scene_to_file", initMap);
				Fader.CallDeferred("crossfade_execute");
			}
			else
			{
				GD.Print("Failed to find INIT map. Unable to start Map Pack.");
			}
		}
		else
		{
			GD.Print("Something went wrong. Mod failed to load.");
		}
	}
}
