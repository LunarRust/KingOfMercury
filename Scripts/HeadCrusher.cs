using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/HeadCrusher.cs")]
public partial class HeadCrusher : Node
{
	[Export(PropertyHint.None, "")]
	public bool basicTerminal;

	[Export(PropertyHint.None, "")]
	public bool finalTerminal;

	private bool used;

	public static int progress;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer headAnim;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer eggAnim;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer terminalAnim;

	[Export(PropertyHint.None, "")]
	public AnimationTree headTree;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	[Export(PropertyHint.None, "")]
	public AudioStream goodSound;

	[Export(PropertyHint.None, "")]
	public AudioStream badSound;

	[Export(PropertyHint.None, "")]
	public Sprite3D sign;

	public int psi = 50;

	public override void _Ready()
	{
		progress = 0;
	}

	public override void _Process(double delta)
	{
	}

	public void Touch()
	{
		if (!used)
		{
			terminalAnim.Play("ShutDown");
			if (basicTerminal)
			{
				progress++;
				GD.Print("Button progress is " + progress);
				soundSource.Stream = goodSound;
				soundSource.Play();
				used = true;
				if (sign != null)
				{
					sign.Frame = 1;
				}
			}
			if (finalTerminal)
			{
				if (progress > 1)
				{
					headCheck();
				}
				else
				{
					GD.Print("Not enough progress!");
				}
			}
		}
		else
		{
			soundSource.Stream = badSound;
			soundSource.Play();
		}
	}

	public void headCheck()
	{
		DialogueBox.instance.Hide();
		if (psi < 500)
		{
			AnimTrigger("KillFail");
			return;
		}
		if (psi > 900)
		{
			AnimTrigger("KillFast");
		}
		else
		{
			AnimTrigger("KillSlow");
		}
		eggAnim.Play("Reveal");
		used = true;
	}

	public async void AnimTrigger(string triggerName)
	{
		headTree.Set("parameters/conditions/" + triggerName, true);
		await Task.Delay(TimeSpan.FromSeconds(0.1));
		if (GodotObject.IsInstanceValid(headTree))
		{
			headTree.Set("parameters/conditions/" + triggerName, false);
		}
	}
}
