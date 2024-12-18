using Godot;

[ScriptPath("res://Scripts/DesertDoor.cs")]
public partial class DesertDoor : Node3D
{
	public int talkCheck;

	[Export(PropertyHint.None, "")]
	public Node doorNode;

	[Export(PropertyHint.None, "")]
	public Node dogNode;

	public void changeInt()
	{
		talkCheck++;
		if (talkCheck > 2)
		{
			doorNode.QueueFree();
			dogNode.QueueFree();
		}
	}
}
