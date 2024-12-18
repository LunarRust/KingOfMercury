using Godot;

[ScriptPath("res://Scripts/tabletpage.cs")]
public partial class tabletpage : Node3D
{
	[Export(PropertyHint.None, "")]
	public Sprite2D pageSprite;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer soundSource;

	private Label closeLabel;

	public MoverTest playerObject;

	public override void _Ready()
	{
		closeLabel = new Label();
		pageSprite.AddChild(closeLabel, forceReadableName: false, InternalMode.Disabled);
		closeLabel.Theme = GD.Load("res://Fonts/DBStyles.tres") as Theme;
		closeLabel.Text = "Press Esc or Spacebar to close.";
		closeLabel.Position = new Vector2(-460f, -255f);
		closeLabel.AddThemeColorOverride("font_color", Colors.DarkRed);
		playerObject = MoverTest.instance;
	}

	public override void _Process(double delta)
	{
		if (pageSprite.Visible && base.GlobalPosition.DistanceTo(playerObject.GlobalPosition) > 2f)
		{
			HidePage();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if ((Input.IsKeyPressed(Godot.Key.Escape) || Input.IsKeyPressed(Godot.Key.Space)) && pageSprite.Visible)
		{
			HidePage();
		}
	}

	public void Touch()
	{
		if (pageSprite.Visible)
		{
			HidePage();
		}
		else
		{
			ShowPage();
		}
	}

	public void Look()
	{
		if (pageSprite.Visible)
		{
			HidePage();
		}
		else
		{
			ShowPage();
		}
	}

	public void ShowPage()
	{
		soundSource.Play();
		hudmanager.HideHUD();
		pageSprite.Show();
	}

	public void HidePage()
	{
		hudmanager.ShowHUD();
		pageSprite.Hide();
	}
}
