using Godot;

[ScriptPath("res://Scripts/DeathScythe.cs")]
public partial class DeathScythe : Area3D
{
	[Export(PropertyHint.None, "")]
	public int damage = 4;

	public override void _Ready()
	{
		base.AreaEntered += playerCheck;
	}

	public void playerCheck(Area3D other)
	{
		if (other.Name == (StringName)"PlayerArea")
		{
			PlayerHealthHandler.instance.changeHealth(-damage);
		}
	}
}
