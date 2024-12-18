using Godot;

[ScriptPath("res://Scripts/NewElevator.cs")]
public partial class NewElevator : Area3D
{
	[Export(PropertyHint.None, "")]
	public PackedScene destinationScene;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	private PackedScene elevatorScene;

	public void OnAreaEntered()
	{
		MoverTest.instance.Set("canMove", false);
		MoverTest.StaticTurn(2);
		anim.Play("Elevator_New");
		elevatorScene = GD.Load("res://Scenes/Elevator.tscn") as PackedScene;
	}

	public void transfer()
	{
		GetTree().ChangeSceneToPacked(elevatorScene);
	}
}
