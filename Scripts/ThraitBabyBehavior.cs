using Godot;

[ScriptPath("res://Scripts/ThraitBabyBehavior.cs")]
public partial class ThraitBabyBehavior : Node
{
	[Export(PropertyHint.None, "")]
	public Node3D[] thraits;

	public void Hurt()
	{
		foreach (Node item in GetTree().GetNodesInGroup("thrait"))
		{
			item.GetNode("Behavior").Call("Hurt");
		}
	}
}
