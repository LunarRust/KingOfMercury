using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/ScreamerCell.cs")]
public partial class ScreamerCell : RigidBody3D
{
	public Node3D playerObject;

	private bool active;

	[Export(PropertyHint.None, "")]
	public float speedLimit = 1f;

	[Export(PropertyHint.None, "")]
	public Area3D aggroTrigger;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	public override void _Ready()
	{
		playerObject = MoverTest.instance;
		base.BodyEntered += resetVelocity;
		aggroTrigger.AreaEntered += aggro;
	}

	public override void _PhysicsProcess(double delta)
	{
		LookAt(playerObject.Position + Vector3.Up * 0.5f, null, useModelFront: true);
		if (active)
		{
			AddConstantForce(base.Transform.Basis.Z * 4f);
			base.LinearVelocity = base.LinearVelocity.Clamp(-Vector3.One * speedLimit, Vector3.One * speedLimit);
		}
	}

	public void resetVelocity(Node body)
	{
		if (active)
		{
			base.ConstantForce = Vector3.Zero;
			if (body.Name == (StringName)"PlayerBody")
			{
				active = false;
				resetActive();
				PlayerHealthHandler.instance.changeHealth(-1);
				AddConstantForce((base.GlobalPosition - playerObject.GlobalPosition) * 200f);
			}
		}
	}

	public async void resetActive()
	{
		await Task.Delay(TimeSpan.FromSeconds(0.5));
		active = true;
	}

	public void aggro(Node area)
	{
		if (area.Name == (StringName)"PlayerArea")
		{
			GD.Print("ScreamCell Has been Triggered!");
			aggroTrigger.QueueFree();
			active = true;
			soundSource.Play();
		}
	}
}
