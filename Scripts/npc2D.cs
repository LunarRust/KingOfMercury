using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/npc2D.cs")]
public partial class npc2D : Node3D
{
	[Export(PropertyHint.None, "")]
	public string[] dialogue;

	private int currentDialogue;

	private bool talking;

	[Export(PropertyHint.None, "")]
	public Area3D dialogueSpace;

	[Export(PropertyHint.None, "")]
	public Label3D label;

	[Export(PropertyHint.None, "")]
	public Sprite3D sprite;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	private RandomNumberGenerator rng;

	public override void _Ready()
	{
		dialogueSpace.AreaEntered += DialogueTrigger;
		rng = new RandomNumberGenerator();
	}

	public override void _Process(double delta)
	{
	}

	public void DialogueTrigger(Area3D area)
	{
		if (area.GlobalPosition.X > base.GlobalPosition.X)
		{
			sprite.FlipH = true;
		}
		else
		{
			sprite.FlipH = false;
		}
		if (!talking)
		{
			DialogueProcessing();
		}
	}

	public async void DialogueProcessing()
	{
		soundSource.PitchScale = rng.RandfRange(0.8f, 1.2f);
		soundSource.Play();
		talking = true;
		label.Text = dialogue[currentDialogue];
		label.Show();
		await Task.Delay(TimeSpan.FromSeconds(4.0));
		label.Hide();
		currentDialogue++;
		if (currentDialogue > dialogue.Length - 1)
		{
			currentDialogue = 0;
		}
		talking = false;
	}
}
