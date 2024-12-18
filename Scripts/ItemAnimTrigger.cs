using Godot;

[ScriptPath("res://Scripts/ItemAnimTrigger.cs")]
public partial class ItemAnimTrigger : Node
{
	[Export(PropertyHint.None, "")]
	public string itemMatch;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public string animationName = "Open";

	private bool opened;

	public bool Item(string item)
	{
		GD.Print("Trying Key");
		GD.Print("Does " + item + " equal " + itemMatch);
		if (item == itemMatch && !opened)
		{
			Open();
			return true;
		}
		return false;
	}

	public void Open()
	{
		anim.Play(animationName);
		QueueFree();
	}
}
