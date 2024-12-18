using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/InventoryCursor.cs")]
public partial class InventoryCursor : Sprite2D
{
	private float speed = 1f;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
		{
			speed = 4f;
		}
		else
		{
			speed = 3f;
		}
		if (Input.GetConnectedJoypads().Count > 0 && InventoryButton.open)
		{
			Vector2 vector = new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));
			if (vector.Length() > 0.2f)
			{
				base.Position += vector * speed;
			}
			base.Position = new Vector2(Mathf.Clamp(base.Position.X, -144f, 145f), Mathf.Clamp(base.Position.Y, 2f, 133f));
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("ControllerAction") && InventoryButton.open)
		{
			CallDeferred("CallMouse");
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
		CursorFollower.changeOnMove = false;
		Input.WarpMouse(playerPos);
		mouseCall.Position = playerPos;
		mouseCall.ButtonIndex = MouseButton.Left;
		mouseCall.Pressed = true;
		Input.ParseInputEvent(mouseCall);
		await Task.Delay(TimeSpan.FromSeconds(0.1));
		CursorFollower.changeOnMove = true;
		Input.ParseInputEvent(mouseCall);
		mouseCall.Pressed = false;
	}
}
