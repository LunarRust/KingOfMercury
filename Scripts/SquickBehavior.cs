using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/SquickBehavior.cs")]
public partial class SquickBehavior : Node3D
{
	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D deathSound;

	[Export(PropertyHint.None, "")]
	public AudioStream scream;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public ColorRect colorRect;

	[Export(PropertyHint.None, "")]
	public Node3D bloodsplat;

	public void OnTaken()
	{
		GD.Print("You took my key!!");
		Explosion();
	}

	public async void Explosion()
	{
		MoverTest.instance.Turn(2f);
		anim.Play("Explode");
		soundSource.Stream = scream;
		soundSource.Play();
		await Task.Delay(TimeSpan.FromSeconds(2.0));
		colorRect.Show();
		PlayerHealthHandler.health = 1;
		PlayerHealthHandler.instance.changeHealth(0);
		bloodsplat.Show();
		CameraShake.Shake(0.5f);
		deathSound.Play();
		await Task.Delay(TimeSpan.FromSeconds(0.10000000149011612));
		GetParent().QueueFree();
	}
}
