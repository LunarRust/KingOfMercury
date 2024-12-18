using Godot;

[ScriptPath("res://Scripts/EarthEgg.cs")]
public partial class EarthEgg : Node
{
	[Export(PropertyHint.None, "")]
	public string LevelName;

	[Export(PropertyHint.None, "")]
	public int mapLevel = 1;

	private GDScript Fader = (GDScript)GD.Load("res://addons/UniversalFade/Fade.gd");

	private GDScript Progress = (GDScript)GD.Load("res://Scripts/game_progress.gd");

	private GDScript Inventory = (GDScript)GD.Load("res://prefabs/InventoryManager.gd");

	private GDScript ACH = (GDScript)GD.Load("res://Scripts/SteamAchievement.gd");

	public void Touch()
	{
		if (!PlayerHealthHandler.usedHeal && LevelName != "Building")
		{
			ACH.Call("_static_grant", "ACH_HEAL");
		}
		if (!CameraCast.usedHammer)
		{
			ACH.Call("_static_grant", "ACH_PEACE");
		}
		Progress.Set("MapLayer", mapLevel);
		Inventory.Call("StoreTrigger");
		Progress.Set(LevelName + "Done", true);
		Progress.Set("keysCollected", (int)Progress.Get("keysCollected") + 1);
		GD.Print(Progress.Get("keysCollected"));
		GD.Print(Progress.Get(LevelName + "Done"));
		Fader.Call("crossfade_prepare", 2, "WeirdWipe", false, false);
		GetTree().CallDeferred("change_scene_to_packed", (PackedScene)GD.Load("res://Scenes/EggGet.tscn"));
		Fader.Call("crossfade_execute");
	}
}
