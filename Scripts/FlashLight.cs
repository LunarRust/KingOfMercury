using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/FlashLight.cs")]
public partial class FlashLight : TextureButton
{
	[Export(PropertyHint.None, "")]
	public Light3D flashlight;

	[Export(PropertyHint.None, "")]
	public Light3D flashlight2;

	[Export(PropertyHint.None, "")]
	private AnimationTree playerAnim;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer soundSource;

	[Export(PropertyHint.None, "")]
	public AudioStream sound;

	public override void _Pressed()
	{
		if (playerAnim != null)
		{
			AnimTrigger("Flashlight");
		}
		flashlight.Visible = !flashlight.Visible;
		flashlight2.Visible = !flashlight2.Visible;
		soundSource.Stream = sound;
		soundSource.Play();
	}

	public async void AnimTrigger(string triggerName)
	{
		playerAnim.Set("parameters/conditions/" + triggerName, true);
		await Task.Delay(TimeSpan.FromSeconds(0.1));
		if (GodotObject.IsInstanceValid(playerAnim))
		{
			playerAnim.Set("parameters/conditions/" + triggerName, false);
		}
	}
}
