using Godot;

[ScriptPath("res://Scripts/DNAConsoleBehavior.cs")]
public partial class DNAConsoleBehavior : Node
{
	[Export(PropertyHint.None, "")]
	public Sprite3D DNAScreen;

	[Export(PropertyHint.None, "")]
	public int Digit = 1;

	private int currentInt;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer soundSource;

	public override void _Ready()
	{
	}

	public void Touch()
	{
		soundSource.Play();
		DNAScreen.Frame = 1;
		if (Digit == 1)
		{
			DNABehavior.lock1 = false;
			return;
		}
		if (Digit == 2)
		{
			DNABehavior.lock2 = false;
		}
		if (Digit == 3)
		{
			DNABehavior.lock3 = false;
		}
	}
}
