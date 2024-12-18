using System.Collections.Generic;
using Godot;
using MEC;

[ScriptPath("res://Scripts/Gib.cs")]
public partial class Gib : RigidBody3D
{
	public override void _Ready()
	{
		RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
		Vector3 force = new Vector3(randomNumberGenerator.RandfRange(-3f, 3f), randomNumberGenerator.RandfRange(2f, 5f), randomNumberGenerator.RandfRange(-3f, 3f));
		AddConstantForce(force);
		Timing.RunCoroutine(Delete());
	}

	private IEnumerator<double> Delete()
	{
		yield return Timing.WaitForSeconds(5.0);
		if (GodotObject.IsInstanceValid(this))
		{
			GD.Print("Deleting Gib");
			QueueFree();
		}
		else
		{
			GD.Print("Gib not found, cannot delete");
		}
	}
}
