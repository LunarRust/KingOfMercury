using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/ShaderCacher.cs")]
public partial class ShaderCacher : Node3D
{
	[Export(PropertyHint.None, "")]
	public PackedScene[] prefabs;

	[Export(PropertyHint.None, "")]
	public string destinationScene;

	private GDScript Fader = (GDScript)GD.Load("res://addons/UniversalFade/Fade.gd");

	public override void _Ready()
	{
		CacheShaders();
	}

	public async void CacheShaders()
	{
		AudioServer.SetBusVolumeDb(0, -80f);
		await Task.Delay(TimeSpan.FromSeconds(1.0));
		PackedScene[] array = prefabs;
		foreach (PackedScene prefab in array)
		{
			Node instance = prefab.Instantiate(PackedScene.GenEditState.Disabled);
			GetNode("/root").CallDeferred("add_child", instance);
		}
		await Task.Delay(TimeSpan.FromSeconds(5.0));
		AudioServer.SetBusVolumeDb(0, 0f);
		GD.Print("Finished Caching Shaders, Moving to Next Scene");
		Fader.CallDeferred("crossfade_prepare", 5, "WeirdWipe", false, false);
		GetTree().CallDeferred("change_scene_to_packed", GD.Load("res://Scenes/" + destinationScene + ".tscn") as PackedScene);
		Fader.CallDeferred("crossfade_execute");
	}
}
