using Godot;

[ScriptPath("res://Scripts/FamilyBehavior.cs")]
public partial class FamilyBehavior : Node
{
	[Export(PropertyHint.None, "")]
	public DesertDoor desertDoor;

	private bool used;

	public void Talk()
	{
		if (!used)
		{
			desertDoor.changeInt();
			used = true;
		}
	}
}
