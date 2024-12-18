using Godot;

[ScriptPath("res://Scripts/Itemdrop.cs")]
public partial class Itemdrop : Node3D
{
	[Export(PropertyHint.None, "")]
	public PackedScene itemObject;

	public void Hurt()
	{
		Node node = itemObject.Instantiate(PackedScene.GenEditState.Disabled);
		GetNode("/root").AddChild(node, forceReadableName: false, InternalMode.Disabled);
		node.Set("position", base.GlobalPosition);
	}
}
