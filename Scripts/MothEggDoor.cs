using Godot;

[ScriptPath("res://Scripts/MothEggDoor.cs")]
public partial class MothEggDoor : StaticBody3D
{
	public static int eggs = 6;

	public static MothEggDoor instance;

	public override void _Ready()
	{
		eggs = 6;
		instance = this;
	}

	public static void killEgg()
	{
		eggs--;
		instance.EggCheck();
	}

	public void EggCheck()
	{
		GD.Print("There are " + eggs + " eggs remaining.");
		if (eggs == 0)
		{
			GD.Print("All eggs killed, destroying egg door");
			QueueFree();
		}
	}
}
