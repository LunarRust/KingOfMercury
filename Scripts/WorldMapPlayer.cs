using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/WorldMapPlayer.cs")]
public partial class WorldMapPlayer : Sprite2D
{
	private float speed = 1f;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
		{
			speed = 2f;
		}
		else
		{
			speed = 1f;
		}
		if (Input.GetConnectedJoypads().Count > 0)
		{
			Vector2 vector = new Vector2(Input.GetJoyAxis(0, JoyAxis.LeftX), Input.GetJoyAxis(0, JoyAxis.LeftY));
			Vector2 vector2 = new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));
			if (vector.Length() > 0.2f || vector2.Length() > 0.2f)
			{
				base.GlobalPosition += vector * speed + vector2 * speed;
			}
			base.GlobalPosition = new Vector2(Mathf.Clamp(base.GlobalPosition.X, 0f, 960f), Mathf.Clamp(base.GlobalPosition.Y, 0f, 540f));
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("ControllerAction"))
		{
			CallDeferred("CallMouse");
		}
		if (@event is InputEventJoypadMotion)
		{
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}
		else if (Input.GetLastMouseVelocity().Length() > 0.1f)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		base._Input(@event);
	}

	public async void CallMouse()
	{
		InputEventMouseButton mouseCall = new InputEventMouseButton();
		Vector2 playerPos = GetGlobalTransformWithCanvas().Origin;
		if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
		{
			playerPos *= 2f;
		}
		GD.Print(playerPos);
		mouseCall.Position = playerPos;
		mouseCall.ButtonIndex = MouseButton.Left;
		mouseCall.Pressed = true;
		Input.ParseInputEvent(mouseCall);
		await Task.Delay(TimeSpan.FromSeconds(0.1));
		Input.ParseInputEvent(mouseCall);
		mouseCall.Pressed = false;
	}
}
