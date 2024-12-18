using System.IO;
using Godot;

[ScriptPath("res://Scripts/ModLoader.cs")]
public partial class ModLoader : Node
{
	private GDScript prefs = (GDScript)GD.Load("res://addons/PlayerPrefs/PlayerPrefs.gd");

	private GDScript ACH = (GDScript)GD.Load("res://Scripts/SteamAchievement.gd");

	private GDScript Workshop = (GDScript)GD.Load("res://Scripts/Workshop.gd");

	[Export(PropertyHint.None, "")]
	public VBoxContainer container;

	[Export(PropertyHint.None, "")]
	public string modPrefabRoot;

	[Export(PropertyHint.None, "")]
	public RichTextLabel noModMessage;

	[Export(PropertyHint.None, "")]
	public CheckBox modLoadBox;

	public PackedScene modPrefab;

	public static bool loadMods;

	public override void _Ready()
	{
		modPrefab = ResourceLoader.Load(modPrefabRoot, "", ResourceLoader.CacheMode.Reuse) as PackedScene;
		GodotObject godotObject = (GodotObject)prefs.New();
		bool flag = (bool)godotObject.Call("get_pref", "loadMods", false);
		if (modLoadBox != null)
		{
			modLoadBox.ButtonPressed = flag;
		}
		if (flag)
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

	public void modSet(bool on)
	{
		GodotObject godotObject = (GodotObject)prefs.New();
		godotObject.Call("set_pref", "loadMods", on);
		GD.Print("Setting mod loading to " + on);
		if (!on)
		{
		}
	}

	public override void _Process(double delta)
	{
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
				ACH.Call("_static_grant", "ACH_MOD");
				string text = "user://mods/" + next;
				noModMessage.Hide();
				Node node = modPrefab.Instantiate(PackedScene.GenEditState.Disabled);
				Label label = node.GetChild(0) as Label;
				Label label2 = node.GetChild(1) as Label;
				Label label3 = node.GetChild(2) as Label;
				TextureRect textureRect = node.GetChild(3) as TextureRect;
				Button button = node.GetChild(4) as Button;
				node.GetChild(5).QueueFree();
				label.Text = next;
				ConfigFile configFile = new ConfigFile();
				int num = 0;
				Error error = configFile.Load(text + "/Info.cfg");
				if (error == Error.Ok)
				{
					label.Text = (string)configFile.GetValue("info", "mod_name");
					label3.Text = (string)configFile.GetValue("info", "mod_description");
					Image image = new Image();
					Error error2 = image.Load(text + "/Icon.png");
					if (error2 == Error.Ok)
					{
						ImageTexture imageTexture = new ImageTexture();
						imageTexture.SetImage(image);
						textureRect.Texture = imageTexture;
					}
					num = (int)configFile.GetValue("info", "mod_type");
					string text2 = (string)configFile.GetValue("info", "mod_directory", "mod.zip");
					node.Set("location", text + "/" + text2);
					switch (num)
					{
					case 0:
						label2.Text = "Mod Type: Resource Mod";
						button.Hide();
						button.ProcessMode = ProcessModeEnum.Disabled;
						GD.Print("Attempting to load resources from " + text + "/" + text2);
						if (ProjectSettings.LoadResourcePack(text + "/" + text2))
						{
							GD.Print("Loading was successful!");
							break;
						}
						GD.Print("Something went wrong. Mod failed to load.");
						label2.Modulate = Color.FromHsv(0f, 89f, 70f);
						label2.Text = "Mod Incompatible";
						label3.Modulate = Color.FromHsv(0f, 89f, 50f);
						label3.Text = "Could not find mod directory defined in config file.";
						textureRect.Texture = GD.Load("res://Sprites/UI/PsychopompFace64Fail.png") as Texture2D;
						button.Hide();
						button.ProcessMode = ProcessModeEnum.Disabled;
						break;
					case 1:
					{
						string text3 = (string)configFile.GetValue("info", "mod_init_map", "res://modmaps/INIT.tscn");
						node.Set("initMap", text3);
						label2.Text = "Mod Type: Map Pack";
						button.Show();
						button.ProcessMode = ProcessModeEnum.Inherit;
						break;
					}
					}
				}
				else
				{
					GD.Print("Could Not Find config file for mod.");
					label2.Modulate = Color.FromHsv(0f, 89f, 70f);
					label2.Text = "Mod Incompatible";
					label3.Modulate = Color.FromHsv(0f, 89f, 50f);
					label3.Text = "Could not find a Config file for this mod. The mod will not run.";
					textureRect.Texture = GD.Load("res://Sprites/UI/PsychopompFace64Fail.png") as Texture2D;
					button.Hide();
					button.ProcessMode = ProcessModeEnum.Disabled;
				}
				container.AddChild(node, forceReadableName: false, InternalMode.Disabled);
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
				ACH.Call("_static_grant", "ACH_MOD");
				string text = path + "\\" + next;
				noModMessage.Hide();
				Node node = modPrefab.Instantiate(PackedScene.GenEditState.Disabled);
				Label label = node.GetChild(0) as Label;
				Label label2 = node.GetChild(1) as Label;
				Label label3 = node.GetChild(2) as Label;
				TextureRect textureRect = node.GetChild(3) as TextureRect;
				Button button = node.GetChild(4) as Button;
				label.Text = next;
				ConfigFile configFile = new ConfigFile();
				int num = 0;
				Error error = configFile.Load(text + "/Info.cfg");
				if (error == Error.Ok)
				{
					label.Text = (string)configFile.GetValue("info", "mod_name");
					label3.Text = (string)configFile.GetValue("info", "mod_description");
					Image image = new Image();
					Error error2 = image.Load(text + "\\Icon.png");
					if (error2 == Error.Ok)
					{
						ImageTexture imageTexture = new ImageTexture();
						imageTexture.SetImage(image);
						textureRect.Texture = imageTexture;
					}
					num = (int)configFile.GetValue("info", "mod_type");
					string text2 = (string)configFile.GetValue("info", "mod_directory", "mod.zip");
					node.Set("location", text + "\\" + text2);
					node.Set("WorkshopId", next);
					switch (num)
					{
					case 0:
						label2.Text = "Mod Type: Resource Mod";
						button.Hide();
						button.ProcessMode = ProcessModeEnum.Disabled;
						GD.Print("Attempting to load resources from " + text + "\\" + text2);
						if (ProjectSettings.LoadResourcePack(text + "\\" + text2))
						{
							GD.Print("Loading was successful!");
							break;
						}
						GD.Print("Something went wrong. Mod failed to load.");
						label2.Modulate = Color.FromHsv(0f, 89f, 70f);
						label2.Text = "Mod Incompatible";
						label3.Modulate = Color.FromHsv(0f, 89f, 50f);
						label3.Text = "Could not find mod directory defined in config file.";
						textureRect.Texture = GD.Load("res://Sprites/UI/PsychopompFace64Fail.png") as Texture2D;
						button.Hide();
						button.ProcessMode = ProcessModeEnum.Disabled;
						break;
					case 1:
					{
						string text3 = (string)configFile.GetValue("info", "mod_init_map", "res://modmaps/INIT.tscn");
						node.Set("initMap", text3);
						label2.Text = "Mod Type: Map Pack";
						button.Show();
						button.ProcessMode = ProcessModeEnum.Inherit;
						break;
					}
					}
				}
				else
				{
					GD.Print("Could Not Find config file for mod.");
					label2.Modulate = Color.FromHsv(0f, 89f, 70f);
					label2.Text = "Mod Incompatible";
					label3.Modulate = Color.FromHsv(0f, 89f, 50f);
					label3.Text = "Could not find a Config file for this mod. The mod will not run.";
					textureRect.Texture = GD.Load("res://Sprites/UI/PsychopompFace64Fail.png") as Texture2D;
					button.Hide();
					button.ProcessMode = ProcessModeEnum.Disabled;
				}
				container.AddChild(node, forceReadableName: false, InternalMode.Disabled);
				next = dirAccess.GetNext();
			}
		}
		else
		{
			GD.Print("An error occurred when trying to access the Workshop path.");
		}
	}

	public void refreshMods()
	{
		foreach (Node child in container.GetChildren())
		{
			child.QueueFree();
		}
		CheckMods("user://mods/");
		string path = (string)Workshop.Call("GetDir");
		DirAccess dirAccess = DirAccess.Open(path);
		if (dirAccess != null)
		{
			CheckWorkshop(path);
		}
	}
}
