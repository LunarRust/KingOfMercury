using Godot;

[ScriptPath("res://Scripts/scenebutton.cs")]
public partial class scenebutton : Node
{
	[Export(PropertyHint.None, "")]
	public string destinationScene;

	private GDScript Fader = (GDScript)GD.Load("res://addons/UniversalFade/Fade.gd");

	[Export(PropertyHint.None, "")]
	public float transitionTime = 2f;

	public void Touch()
	{
		transfer();
	}

	public void Look()
	{
		transfer();
	}

	public void Hurt()
	{
		transfer();
	}

	public void Talk()
	{
		transfer();
	}

	public void transfer()
	{
		Fader.CallDeferred("crossfade_prepare", transitionTime, "WeirdWipe", false, false);
		GetTree().CallDeferred("change_scene_to_packed", GD.Load("res://Scenes/" + destinationScene + ".tscn") as PackedScene);
		Fader.CallDeferred("crossfade_execute");
	}
}
