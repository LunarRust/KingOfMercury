using Godot;

[ScriptPath("res://Scripts/EnemyHealthHandler.cs")]
public partial class EnemyHealthHandler : Node3D
{
	private GDScript ACH = (GDScript)GD.Load("res://Scripts/SteamAchievement.gd");

	[Export(PropertyHint.None, "")]
	public int HP = 1;

	private bool dead;

	[Export(PropertyHint.None, "")]
	public bool Innocent = false;

	[Export(PropertyHint.None, "")]
	public string gibRoot = "res://prefabs/blood_splatter.tscn";

	[Export(PropertyHint.None, "")]
	public string gibRoot2 = "res://prefabs/blood_splatter2.tscn";

	public PackedScene gib;

	public PackedScene gib2;

	public override void _Ready()
	{
		gib = ResourceLoader.Load(gibRoot, "", ResourceLoader.CacheMode.Reuse) as PackedScene;
		gib2 = ResourceLoader.Load(gibRoot2, "", ResourceLoader.CacheMode.Reuse) as PackedScene;
	}

	public override void _Process(double delta)
	{
		if (HP < 1 && !dead)
		{
			Death();
			dead = true;
		}
	}

	public void Hurt(int amount)
	{
		if (HP > 1)
		{
			Node node = gib2.Instantiate(PackedScene.GenEditState.Disabled);
			GetNode("/root").AddChild(node, forceReadableName: false, InternalMode.Disabled);
			node.Set("position", base.GlobalPosition);
		}
		GD.Print("Hurt for " + amount);
		HP -= amount;
		CameraShake.Shake(0.08f);
	}

	public void Death()
	{
		ACH.Call("_static_grant", "ACH_KILL");
		CameraShake.Shake(0.15f);
		DialogueBox.instance.Hide();
		Node node = gib.Instantiate(PackedScene.GenEditState.Disabled);
		GetNode("/root").AddChild(node, forceReadableName: false, InternalMode.Disabled);
		node.Set("position", base.GlobalPosition);
		GetParent().QueueFree();
	}
}
