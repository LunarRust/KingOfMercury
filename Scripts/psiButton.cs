using Godot;

[ScriptPath("res://Scripts/psiButton.cs")]
public partial class psiButton : Node
{
	[Export(PropertyHint.None, "")]
	public bool positive;

	[Export(PropertyHint.None, "")]
	public Label3D label;

	[Export(PropertyHint.None, "")]
	public HeadCrusher headCrusher;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	[Export(PropertyHint.None, "")]
	public Sprite3DShake shaker;

	public override void _Ready()
	{
	}

	public void Touch()
	{
		if (positive)
		{
			psiChange(50);
		}
		else
		{
			psiChange(-50);
		}
	}

	public void psiChange(int amount)
	{
		headCrusher.psi += amount;
		headCrusher.psi = Mathf.Clamp(headCrusher.psi, 0, 999);
		label.Text = "PSI: " + headCrusher.psi;
		soundSource.PitchScale = 1 + headCrusher.psi / 100;
		shaker.power = (float)headCrusher.psi / 10000f;
	}
}
