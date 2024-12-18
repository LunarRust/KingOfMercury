using Godot;

[ScriptPath("res://Scripts/SceneTransfer.cs")]
public partial class SceneTransfer : Area3D
{
	[Export(PropertyHint.None, "")]
	public bool crossFade = true;

	[Export(PropertyHint.None, "")]
	public bool changePosition;

	[Export(PropertyHint.None, "")]
	public float transitionTime = 2f;

	[Export(PropertyHint.None, "")]
	public Vector3 targetPosition;

	public static bool setPosition;

	public static Vector3 endPosition;

	[Export(PropertyHint.None, "")]
	public string destinationScene;

	[Export(PropertyHint.None, "")]
	public string altScene;

	private GDScript Fader = (GDScript)GD.Load("res://addons/UniversalFade/Fade.gd");

	private Node3D playerNode;

	public override void _Ready()
	{
		base.AreaEntered += OnBodyEntered;
		playerNode = MoverTest.instance;
	}

	private void OnBodyEntered(Node3D playerNode)
	{
		if (playerNode != null && playerNode.Name == (StringName)"PlayerArea")
		{
			GD.Print(string.Concat(playerNode.Name, " has entered a transfer node!"));
			SceneChange();
		}
		else if (playerNode == null)
		{
			SceneChange();
		}
	}

	public void SceneChange()
	{
		if (changePosition)
		{
			endPosition = targetPosition;
			setPosition = true;
		}
		if (crossFade)
		{
			Fader.CallDeferred("crossfade_prepare", transitionTime, "WeirdWipe", false, false);
		}
		if (destinationScene != null)
		{
			GetTree().CallDeferred("change_scene_to_packed", GD.Load("res://Scenes/" + destinationScene + ".tscn") as PackedScene);
		}
		else
		{
			GetTree().CallDeferred("change_scene_to_packed", GD.Load(altScene) as PackedScene);
		}
		if (crossFade)
		{
			Fader.CallDeferred("crossfade_execute");
		}
	}
}
