using Godot;

[ScriptPath("res://Scripts/BasicEnemy.cs")]
public partial class BasicEnemy : Node3D
{
	[Export(PropertyHint.None, "")]
	public Node StateMachine;

	[Export(PropertyHint.None, "")]
	public NavigationAgent3D navAgent;

	[Export(PropertyHint.None, "")]
	public CharacterBody3D cha;

	public MoverTest playerObject;

	private Vector3 homePoint;

	public override void _Ready()
	{
		playerObject = MoverTest.instance;
	}

	public override void _Process(double delta)
	{
		if (cha.Position.DistanceTo(playerObject.Position) > 1f)
		{
			cha.LookAt(playerObject.Position, Vector3.Up, useModelFront: true);
			cha.Velocity = cha.Transform.Basis.Z;
			cha.MoveAndSlide();
		}
	}

	private void _on_state_machine_player_updated(string state, double delta)
	{
		if (state == "Idle")
		{
			StateMachine.Call("set_trigger", "Chase");
		}
		if (state == "Chase")
		{
			StateMachine.Call("set_trigger", "Idle");
		}
		if (!(state == "Dead"))
		{
		}
	}

	private void Idle()
	{
	}

	private void Chase()
	{
	}

	private void Dead()
	{
	}
}
