using Godot;

[ScriptPath("res://Scripts/InteractionButtonMaster.cs")]
public partial class InteractionButtonMaster : Sprite2D
{
	public static int currentMode;

	[Export(PropertyHint.None, "")]
	public TextureButton[] buttons;

	[Export(PropertyHint.None, "")]
	public Sprite2D blipSprite;

	public override void _Ready()
	{
		currentMode = 0;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ModeUp"))
		{
			currentMode++;
			modeCheck();
		}
		if (Input.IsActionJustPressed("ModeDown"))
		{
			currentMode--;
			modeCheck();
		}
		if (Input.IsMouseButtonPressed(MouseButton.None) && blipSprite != null)
		{
			blipSprite.Visible = false;
		}
	}

	public void modeCheck()
	{
		if (blipSprite != null)
		{
			blipSprite.Visible = true;
		}
		if (currentMode < 0)
		{
			currentMode = 3;
		}
		else if (currentMode > 3)
		{
			currentMode = 0;
		}
		GD.Print("Switching interaction mode to button: " + currentMode);
		buttons[currentMode]._Pressed();
	}
}
