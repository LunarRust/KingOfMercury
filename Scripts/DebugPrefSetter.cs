using Godot;

[ScriptPath("res://Scripts/DebugPrefSetter.cs")]
public partial class DebugPrefSetter : Node
{
	private GDScript prefs = (GDScript)GD.Load("res://addons/PlayerPrefs/PlayerPrefs.gd");

	[Export(PropertyHint.None, "")]
	private int eggSet;

	[Export(PropertyHint.None, "")]
	private bool finishgame;

	[Export(PropertyHint.None, "")]
	private bool library;

	[Export(PropertyHint.None, "")]
	private int mapLevel;

	public override void _Ready()
	{
		GodotObject godotObject = (GodotObject)prefs.New();
		godotObject.Call("set_pref", "Keys", eggSet);
		godotObject.Call("set_pref", "WonGame", finishgame);
		godotObject.Call("set_pref", "Library", library);
		godotObject.Call("set_pref", "MapLayer", mapLevel);
	}
}
