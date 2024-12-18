using Godot;

[ScriptPath("res://Scripts/DialogueBox.cs")]
public partial class DialogueBox : Sprite2D
{
	public static DialogueBox instance;

	public override void _Ready()
	{
		instance = this;
	}

	public override void _Input(InputEvent @event)
	{
		if ((Input.IsKeyPressed(Godot.Key.Escape) || Input.IsKeyPressed(Godot.Key.Space) || Input.IsJoyButtonPressed(0, JoyButton.B)) && instance.Visible)
		{
			instance.Hide();
		}
	}
}
