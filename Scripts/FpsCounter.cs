using Godot;

[ScriptPath("res://Scripts/FpsCounter.cs")]
public partial class FpsCounter : Label
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Debug"))
		{
			base.Visible = !base.Visible;
		}
		base.Text = "FPS: " + Engine.GetFramesPerSecond();
	}
}
