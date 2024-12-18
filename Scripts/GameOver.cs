using Godot;

[ScriptPath("res://Scripts/GameOver.cs")]
public partial class GameOver : Node
{
	[Export(PropertyHint.None, "")]
	public bool canChange;

	private GDScript Fader = (GDScript)GD.Load("res://addons/UniversalFade/Fade.gd");

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if ((Input.IsActionJustPressed("MouseAction") || Input.IsActionJustPressed("ControllerAction")) && canChange)
		{
			Fader.CallDeferred("crossfade_prepare", 3, "WeirdWipe", false, false);
			GetTree().CallDeferred("change_scene_to_file", "res://Scenes/title_screen.tscn");
			Fader.CallDeferred("crossfade_execute");
			base._Input(@event);
		}
	}
}
