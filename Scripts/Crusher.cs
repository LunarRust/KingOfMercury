using Godot;
using Godot.Collections;

[ScriptPath("res://Scripts/Crusher.cs")]
public partial class Crusher : Area3D
{
	[Export(PropertyHint.None, "")]
	public Node3D crusherModel;

	public MoverTest playerObject;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer deathAnim;

	private GDScript ACH = (GDScript)GD.Load("res://Scripts/SteamAchievement.gd");

	public override void _Ready()
	{
		playerObject = MoverTest.instance;
	}

	public void triggerShake()
	{
		if (base.Position.DistanceTo(playerObject.Position) < 6f)
		{
			CameraShake.Shake(0.25f);
		}
	}

	public void DamageCheck()
	{
		Array<Area3D> overlappingAreas = GetOverlappingAreas();
		foreach (Area3D item in overlappingAreas)
		{
			if (item.Name == (StringName)"PlayerArea")
			{
				ACH.Call("_static_grant", "ACH_CRUSHER");
				deathAnim.Stop();
				deathAnim.Play("CrusherDeath");
				Vector3 vector = new Vector3(23.5f, -2f, -40.5f);
				playerObject.Position = vector;
				playerObject.newPos = vector;
				PlayerHealthHandler.instance.changeHealth(-3);
			}
		}
	}
}
