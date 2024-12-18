using Godot;

[ScriptPath("res://Scenes/KeyLockTest.cs")]
public partial class KeyLockTest : Node
{
	[Export(PropertyHint.None, "")]
	public string itemMatch;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public bool Item(string item)
	{
		if (item == itemMatch)
		{
			GD.Print("That's the item!");
			GetParent().QueueFree();
			return true;
		}
		GD.Print("That's not the item...");
		return false;
	}
}
