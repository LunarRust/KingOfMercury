using Godot;

[ScriptPath("res://Scripts/ViewButton.cs")]
public partial class ViewButton : TextureButton
{
	[Export(PropertyHint.None, "")]
	public Node3D viewPos1;

	[Export(PropertyHint.None, "")]
	public Node3D viewPos2;

	public static bool thirdActive;

	public static bool unlocked;

	public Camera3D mainCamera;

	[Export(PropertyHint.None, "")]
	public SpringArm3D cameraParent;

	[Export(PropertyHint.None, "")]
	public SpringArm3D cameraParentHorizontal;

	[Export(PropertyHint.None, "")]
	private Node2D uiElements;

	[Export(PropertyHint.None, "")]
	private Node2D uiElements2;

	[Export(PropertyHint.None, "")]
	private Node2D uiHammer;

	public override void _Ready()
	{
		mainCamera = GetViewport().GetCamera3D();
		base.Pressed += ViewCheck;
		if (thirdActive)
		{
			setThird();
		}
	}

	public override void _Process(double delta)
	{
	}

	public void ViewCheck()
	{
		if (thirdActive)
		{
			setFirst();
		}
		else
		{
			setThird();
		}
	}

	public void setThird()
	{
		cameraParent.SpringLength = 0.9f;
		thirdActive = true;
		uiElements.Hide();
		uiHammer.Hide();
		uiElements2.Position = new Vector2(uiElements2.Position.X + 150f, uiElements2.Position.Y);
		cameraParent.Position = viewPos2.Position;
		mainCamera.SetCullMaskValue(5, value: true);
		mainCamera.SetCullMaskValue(6, value: false);
	}

	public void setFirst()
	{
		cameraParent.SpringLength = 0f;
		thirdActive = false;
		uiElements.Show();
		uiHammer.Show();
		uiElements2.Position = new Vector2(uiElements2.Position.X - 150f, uiElements2.Position.Y);
		cameraParent.Position = viewPos1.Position;
		mainCamera.SetCullMaskValue(5, value: false);
		mainCamera.SetCullMaskValue(6, value: true);
	}
}
