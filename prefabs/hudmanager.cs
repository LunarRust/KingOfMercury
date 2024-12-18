using Godot;

[ScriptPath("res://prefabs/hudmanager.cs")]
public partial class hudmanager : CanvasLayer
{
	public static hudmanager instance;

	[Export(PropertyHint.None, "")]
	public ColorRect backupFilter;

	public override void _Ready()
	{
		instance = this;
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("HideHud"))
		{
			if (instance.Visible)
			{
				HideHUD();
			}
			else
			{
				ShowHUD();
			}
		}
	}

	public static void HideHUD()
	{
		instance.Hide();
		if (instance.backupFilter != null)
		{
			instance.backupFilter.Show();
		}
	}

	public static void ShowHUD()
	{
		instance.Show();
		if (instance.backupFilter != null)
		{
			instance.backupFilter.Hide();
		}
	}
}
