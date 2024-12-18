using Godot;

[ScriptPath("res://Scripts/ParagonBehavior.cs")]
public partial class ParagonBehavior : Node3D
{
	[Export(PropertyHint.None, "")]
	public string aspect = "Aspect";

	[Export(PropertyHint.None, "")]
	public AnimationPlayer labelPlayer;

	[Export(PropertyHint.None, "")]
	public RichTextLabel label;

	[Export(PropertyHint.None, "")]
	public Itemdrop item;

	[Export(PropertyHint.None, "")]
	public Area3D teleporter;

	public override void _Ready()
	{
		if (teleporter != null)
		{
			teleporter.Monitoring = false;
			teleporter.Hide();
		}
	}

	public override void _Process(double delta)
	{
	}

	public void Hurt()
	{
		if (teleporter != null)
		{
			teleporter.Monitoring = true;
			teleporter.Show();
		}
		item.Hurt();
		label.Text = "[wave][center]You finally understand " + aspect;
		labelPlayer.Play("Fade");
	}
}
