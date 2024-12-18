using Godot;

[ScriptPath("res://Scripts/DispersalMachineBehavior.cs")]
public partial class DispersalMachineBehavior : Node
{
	[Export(PropertyHint.None, "")]
	public string[] itemMatch;

	private int progress;

	[Export(PropertyHint.None, "")]
	public GpuParticles3D particles;

	[Export(PropertyHint.None, "")]
	public Node3D MothParent;

	[Export(PropertyHint.None, "")]
	public DialogueSystem NPCDialogueSystem;

	[Export(PropertyHint.None, "")]
	public DialogueSystem NPCDialogueSystem2;

	[Export(PropertyHint.None, "")]
	public string NPCDialogue;

	[Export(PropertyHint.None, "")]
	public string NPCDialogue2;

	[Export(PropertyHint.None, "")]
	private AudioStreamPlayer soundSource;

	[Export(PropertyHint.None, "")]
	public Node3D Sphincter;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public bool Item(string item)
	{
		string[] array = itemMatch;
		foreach (string text in array)
		{
			GD.Print("Does " + item + " equal " + text);
			if (item == text)
			{
				soundSource.Play();
				progress++;
				if (progress == 3)
				{
					Complete();
				}
				return true;
			}
		}
		return false;
	}

	public void Complete()
	{
		hudmanager.HideHUD();
		anim.Play("KillMoths");
		Sphincter.GetNode<AnimationPlayer>("AnimationPlayer").Play("Open");
		Sphincter.GetNode<CollisionShape3D>("StaticBody3D/CollisionShape3D").Disabled = true;
		NPCDialogueSystem.Dialogue = NPCDialogue;
		NPCDialogueSystem2.Dialogue = NPCDialogue2;
	}

	public void killMoths()
	{
		MothParent.Hide();
	}
}
