using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/TotallerBehavior.cs")]
public partial class TotallerBehavior : Node3D
{
	[Export(PropertyHint.None, "")]
	public AnimationTree anim;

	public float anger = 0f;

	[Export(PropertyHint.None, "")]
	public Node3D EnemyAI;

	private Node3D playerObject;

	private bool aggro;

	private bool hit;

	public override void _Ready()
	{
		playerObject = GetTree().GetFirstNodeInGroup("player") as Node3D;
		anim.AnimationFinished += startAttack;
	}

	public override void _Process(double delta)
	{
		if (base.GlobalPosition.DistanceTo(playerObject.GlobalPosition) < 6f && aggro)
		{
			anger += (float)delta;
		}
		if (anger > 2f)
		{
			anger = 0f;
			EnemyAI.Set("attacking", false);
			AnimTrigger("Attack");
			aggro = false;
		}
	}

	public void startAttack(StringName name)
	{
		GD.Print(name.ToString());
		if (name.ToString() == "Roar" || name.ToString() == "FireBlow")
		{
			EnemyAI.Set("attacking", true);
			aggro = true;
		}
	}

	public void Hurt()
	{
		if (!hit)
		{
			hit = true;
			GD.Print("Hurt the Totaller");
			AnimTrigger("Hurt");
		}
	}

	public async void AnimTrigger(string triggerName)
	{
		anim.Set("parameters/conditions/" + triggerName, true);
		await Task.Delay(TimeSpan.FromSeconds(0.1));
		if (GodotObject.IsInstanceValid(anim))
		{
			anim.Set("parameters/conditions/" + triggerName, false);
		}
	}
}
