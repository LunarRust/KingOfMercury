using Godot;

[ScriptPath("res://Scripts/Usebutton.cs")]
public partial class Usebutton : TextureButton
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventJoypadMotion && InventoryButton.open && CursorFollower.changeOnMove)
		{
			base.Visible = true;
		}
		else if (@event is InputEventMouseMotion && CursorFollower.changeOnMove)
		{
			base.Visible = false;
		}
		base._Input(@event);
	}
}
