using System.IO;
using Godot;

[ScriptPath("res://Scenes/initScreenLoader.cs")]
public partial class initScreenLoader : Node
{
	private GDScript prefs = (GDScript)GD.Load("res://addons/PlayerPrefs/PlayerPrefs.gd");

	private GDScript Workshop = (GDScript)GD.Load("res://Scripts/Workshop.gd");

	public override void _Ready()
	{
		GodotObject godotObject = (GodotObject)prefs.New();
		if ((bool)godotObject.Call("get_pref", "loadMods", false))
		{
			GD.Print("Mod Loading is currently On. Loading Mods now.");
			CheckMods("user://mods/");
			string path = (string)Workshop.Call("GetDir");
			DirAccess dirAccess = DirAccess.Open(path);
			if (dirAccess != null)
			{
				CheckWorkshop(path);
			}
		}
		else
		{
			GD.Print("Mod Loading is currently off. Please check the mod menu for more details.");
		}
	}

	public void CheckMods(string path)
	{
		using DirAccess dirAccess = DirAccess.Open(path);
		if (dirAccess != null)
		{
			dirAccess.ListDirBegin();
			string next = dirAccess.GetNext();
			while (next != "")
			{
				string text = "user://mods/" + next;
				ConfigFile configFile = new ConfigFile();
				int num = 0;
				Error error = configFile.Load(text + "/Info.cfg");
				if (error == Error.Ok)
				{
					num = (int)configFile.GetValue("info", "mod_type");
					string text2 = (string)configFile.GetValue("info", "mod_directory", "mod.zip");
					if (num == 0)
					{
						GD.Print("Attempting to load resources from " + text + "/" + text2);
						if (ProjectSettings.LoadResourcePack(text + "/" + text2))
						{
							GD.Print("Loading was successful!");
						}
						else
						{
							GD.Print("Something went wrong. Mod failed to load.");
						}
					}
				}
				else
				{
					GD.Print("Could Not Find config file for mod.");
				}
				next = dirAccess.GetNext();
			}
		}
		else
		{
			GD.Print("An error occurred when trying to access the path. Creating the path and trying again.");
			Directory.CreateDirectory(OS.GetUserDataDir() + "/mods/");
			CheckMods("user://mods/");
		}
	}

	public void CheckWorkshop(string path)
	{
		using DirAccess dirAccess = DirAccess.Open(path);
		if (dirAccess != null)
		{
			dirAccess.ListDirBegin();
			string next = dirAccess.GetNext();
			while (next != "")
			{
				string text = path + "\\" + next;
				ConfigFile configFile = new ConfigFile();
				int num = 0;
				Error error = configFile.Load(text + "/Info.cfg");
				if (error == Error.Ok)
				{
					num = (int)configFile.GetValue("info", "mod_type");
					string text2 = (string)configFile.GetValue("info", "mod_directory", "mod.zip");
					switch (num)
					{
					case 0:
						GD.Print("Attempting to load resources from " + text + "\\" + text2);
						if (ProjectSettings.LoadResourcePack(text + "\\" + text2))
						{
							GD.Print("Loading was successful!");
						}
						break;
					}
				}
				else
				{
					GD.Print("Could Not Find config file for mod.");
				}
				next = dirAccess.GetNext();
			}
		}
		else
		{
			GD.Print("An error occurred when trying to access the Workshop path.");
		}
	}
}
