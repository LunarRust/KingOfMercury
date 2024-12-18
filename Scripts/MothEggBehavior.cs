using Godot;

[ScriptPath("res://Scripts/MothEggBehavior.cs")]
public partial class MothEggBehavior : Node
{
	public void Hurt()
	{
		MothEggDoor.killEgg();
	}
}
