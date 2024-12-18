using Godot;

[ScriptPath("res://Scripts/TeratomaBehavior.cs")]
public partial class TeratomaBehavior : Node
{
	[Export(PropertyHint.None, "")]
	public Node syringeItem;

	public bool Item(string item)
	{
		if (item == "Empty Syringe")
		{
			GD.Print("It's the empty Syringe!");
			syringeItem.Call("Touch");
			return true;
		}
		GD.Print("That's wrong, dude!");
		return false;
	}
}
