using Godot;

[ScriptPath("res://Scripts/CausticRegion.cs")]
public partial class CausticRegion : Area3D
{
	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer soundSource;

	[Export(PropertyHint.None, "")]
	public int amount = -1;

	private bool active;

	private float timer;

	public override void _Ready()
	{
		base.AreaEntered += OnAreaEntered;
		base.AreaExited += OnAreaExited;
	}

	public override void _Process(double delta)
	{
		if (active)
		{
			timer += (float)delta;
		}
		else
		{
			timer = 0f;
		}
		if (timer > 3f)
		{
			PlayerHealthHandler.instance.changeHealth(amount);
			timer = 0f;
		}
	}

	public void OnAreaEntered(Area3D area)
	{
		if (area.Name == (StringName)"PlayerArea")
		{
			active = true;
			if (soundSource != null)
			{
				soundSource.Play();
			}
		}
	}

	public void OnAreaExited(Area3D area)
	{
		if (area.Name == (StringName)"PlayerArea")
		{
			active = false;
			if (soundSource != null)
			{
				soundSource.Stop();
			}
		}
	}
}
