using Godot;

[ScriptPath("res://Scripts/TaprootBehavior.cs")]
public partial class TaprootBehavior : Node
{
	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public DialogueSystem dialogue;

	public void Hurt()
	{
		anim.Play("BuildingsDie");
		dialogue.Dialogue = "What have you done...";
	}
}
