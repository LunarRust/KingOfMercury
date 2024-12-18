using Godot;

[ScriptPath("res://Scripts/basicProjectile.cs")]
public partial class basicProjectile : Area3D
{
	[Export(PropertyHint.None, "")]
	public float speed = 5f;

	[Export(PropertyHint.None, "")]
	public float lifeTime = 5f;

	public override void _Ready()
	{
		base.AreaEntered += collisionTest;
		base.BodyEntered += OtherCollision;
	}

	public override void _Process(double delta)
	{
		base.Position += base.Transform.Basis.Z * (float)delta * speed;
		lifeTime -= (float)delta;
		if (lifeTime < 0f)
		{
			QueueFree();
		}
	}

	public void collisionTest(Area3D area)
	{
		if (area.Name == (StringName)"PlayerArea")
		{
			PlayerHealthHandler.instance.changeHealth(-2);
		}
		QueueFree();
	}

	public void OtherCollision(Node3D body)
	{
		QueueFree();
	}
}
