using System;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

[ScriptPath("res://Scripts/TurretEnemy.cs")]
public partial class TurretEnemy : Node3D
{
	[Export(PropertyHint.None, "")]
	public Node3D playerObject;

	private Vector3 playerPosition;

	[Export(PropertyHint.None, "")]
	public int aggroRange = 5;

	[Export(PropertyHint.None, "")]
	private bool aggro;

	private float timer;

	[Export(PropertyHint.None, "")]
	public Node3D shotPoint;

	[Export(PropertyHint.None, "")]
	public string projectilePath;

	private PackedScene projectileObject;

	[Export(PropertyHint.None, "")]
	public int projectileInterval;

	[Export(PropertyHint.None, "")]
	public AnimationTree anim;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	public override void _Ready()
	{
		projectileObject = GD.Load<PackedScene>(projectilePath);
		if (playerObject == null)
		{
			GD.Print("No player object was set manually. Looking for one automatically.");
			playerObject = GetTree().GetFirstNodeInGroup("player") as Node3D;
		}
	}

	public override void _Process(double delta)
	{
		if (base.GlobalPosition.DistanceTo(playerObject.GlobalPosition) < (float)aggroRange)
		{
			aggro = true;
		}
		else
		{
			aggro = false;
		}
		if (aggro)
		{
			playerPosition = new Vector3(playerObject.GlobalPosition.X, base.GlobalPosition.Y, playerObject.GlobalPosition.Z);
			LookAt(playerPosition, Vector3.Up, useModelFront: true);
			timer += (float)delta;
			if (timer > (float)projectileInterval)
			{
				shootTest();
				timer = 0f;
			}
		}
	}

	public async void shoot()
	{
		soundSource.Play();
		if (GodotObject.IsInstanceValid(anim))
		{
			anim.Set("parameters/conditions/Attack", true);
		}
		GD.Print(string.Concat(base.Name, " is Shooting Projectile!"));
		Node3D instance = projectileObject.Instantiate<Node3D>(PackedScene.GenEditState.Disabled);
		GetNode("/root").AddChild(instance, forceReadableName: false, InternalMode.Disabled);
		instance.GlobalPosition = shotPoint.GlobalPosition;
		instance.GlobalRotation = shotPoint.GlobalRotation;
		await Task.Delay(TimeSpan.FromSeconds(0.2));
		if (GodotObject.IsInstanceValid(anim))
		{
			anim.Set("parameters/conditions/Attack", false);
		}
	}

	public void shootTest()
	{
		GD.Print("Sending Query for Player");
		PhysicsDirectSpaceState3D directSpaceState = GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D parameters = PhysicsRayQueryParameters3D.Create(shotPoint.GlobalPosition, playerObject.GlobalPosition);
		Dictionary dictionary = directSpaceState.IntersectRay(parameters);
		if (dictionary.Count > 0 && dictionary["collider"].As<CollisionObject3D>().Name == (StringName)"PlayerBody")
		{
			GD.Print("Player was seen. Shooting Projectile.");
			shoot();
		}
	}
}
