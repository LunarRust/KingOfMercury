using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/DNATester.cs")]
public partial class DNATester : Node
{
	private RandomNumberGenerator rng;

	private bool testing;

	[Export(PropertyHint.None, "")]
	public Sprite3D screen1;

	[Export(PropertyHint.None, "")]
	public Sprite3D screen2;

	[Export(PropertyHint.None, "")]
	public Sprite3D screen3;

	[Export(PropertyHint.None, "")]
	public Sprite3D screen4;

	[Export(PropertyHint.None, "")]
	public Node3D syringe;

	[Export(PropertyHint.None, "")]
	public Label3D DNAlabel;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer hookAnim;

	[Export(PropertyHint.None, "")]
	public DialogueSystem alleleDialogue;

	public override void _Ready()
	{
		rng = new RandomNumberGenerator();
	}

	public override void _Process(double delta)
	{
		if (testing)
		{
			screen1.Frame = rng.RandiRange(0, 3);
			screen2.Frame = rng.RandiRange(0, 3);
			screen3.Frame = rng.RandiRange(0, 3);
			screen4.Frame = rng.RandiRange(0, 3);
		}
	}

	public bool Item(string item)
	{
		if (item == "Filled Syringe")
		{
			DNAProcess();
			return true;
		}
		return false;
	}

	public async void DNAProcess()
	{
		DNAlabel.Text = "Testing DNA Sample";
		testing = true;
		syringe.Show();
		await Task.Delay(TimeSpan.FromSeconds(5.0));
		testing = false;
		screen1.Frame = 3;
		screen2.Frame = 1;
		screen3.Frame = 0;
		screen4.Frame = 2;
		DNAlabel.Text = "Testing Complete";
		hookAnim.Play("Hook Down");
		alleleDialogue.Dialogue = "Hey, so you got the DNA Sample, right? Good, good. That means we can cook something up that'll really hurt this clog. It's just a matter of finding the right corruption, eh?";
	}
}
