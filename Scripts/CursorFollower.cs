using Godot;

[ScriptPath("res://Scripts/CursorFollower.cs")]
public partial class CursorFollower : Node2D
{
	public static bool changeOnMove = true;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse inputEventMouse)
		{
			base.Position = inputEventMouse.Position + new Vector2(20f, 20f);
		}
		if (@event is InputEventJoypadMotion && changeOnMove)
		{
			base.Visible = false;
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}
		else if (@event is InputEventMouseMotion && changeOnMove)
		{
			base.Visible = true;
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		base._Input(@event);
	}
}
