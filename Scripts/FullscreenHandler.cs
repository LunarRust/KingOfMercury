using Godot;

[ScriptPath("res://Scripts/FullscreenHandler.cs")]
public partial class FullscreenHandler : Node
{
	public override void _Ready()
	{
		GD.Print("Initializing Fullscreen Checker");
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("FullScreen"))
		{
			if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				GetWindow().Size = new Vector2I(960, 540);
				GetWindow().Position = new Vector2I(480, 270);
			}
			else
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
			}
		}
	}
}
