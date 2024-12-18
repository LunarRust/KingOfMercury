using Godot;

[ScriptPath("res://Scripts/KoadBehavior.cs")]
public partial class KoadBehavior : Node
{
	public int talkedto = 0;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	[Export(PropertyHint.None, "")]
	public AudioStream openSound;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public DialogueSystem dialogue;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer ACH;

	public void Talk()
	{
		if (talkedto == 5)
		{
			soundSource.Stream = openSound;
			soundSource.Play();
			anim.Play("Splitting");
			dialogue.npcName = "King of all Dogs";
			dialogue.Dialogue = "Don't idle here. You have much to do! Don't worry, for there is a plan to everything. I'm sure you will see in due time, my friend.";
			dialogue.LookDescription = "The dog has bloomed wonderfully!";
			dialogue.TouchDescription = "Ground Beef Feeling.";
			talkedto++;
		}
		else if (talkedto < 5)
		{
			talkedto++;
			dialogue.Dialogue = "Ruff!";
		}
	}

	public void Touch()
	{
		ACH.Play("ACH");
	}
}
