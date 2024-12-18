using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/EmeraldKeyLock.cs")]
public partial class EmeraldKeyLock : StaticBody3D
{
	[Export(PropertyHint.None, "")]
	public int requirement;

	[Export(PropertyHint.None, "")]
	public Area3D trigger;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	private GDScript Progress = (GDScript)GD.Load("res://Scripts/game_progress.gd");

	private bool open;

	public override void _Ready()
	{
		trigger.AreaEntered += hitCheck;
		if (requirement <= (int)Progress.Get("EmeraldKeys"))
		{
			open = true;
		}
	}

	public override void _Process(double delta)
	{
	}

	public void hitCheck(Node other)
	{
		if (other.Name == (StringName)"PlayerArea" && open)
		{
			erase();
		}
		else if (other.Name == (StringName)"PlayerArea")
		{
			GD.Print("Not Enough Keys");
		}
	}

	public async void erase()
	{
		soundSource.Play();
		anim.Play("Hide");
		await Task.Delay(TimeSpan.FromSeconds(1.2));
		QueueFree();
	}
}
