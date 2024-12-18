using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/BelugaBehavior.cs")]
public partial class BelugaBehavior : Node3D
{
	[Export(PropertyHint.None, "")]
	public AnimationTree anim;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer attackAnim;

	private float anger;

	private bool aggro;

	private bool triggered;

	private Node3D playerObject;

	[Export(PropertyHint.None, "")]
	public Node3D EnemyAI;

	public override void _Ready()
	{
		anim.AnimationFinished += animEnd;
		attackAnim.AnimationFinished += animEnd;
		playerObject = GetTree().GetFirstNodeInGroup("player") as Node3D;
	}

	public override void _Process(double delta)
	{
		if (base.GlobalPosition.DistanceTo(playerObject.GlobalPosition) < 15f && !triggered)
		{
			EnemyAI.Set("attacking", true);
			AnimTrigger("walking");
			aggro = true;
			triggered = true;
		}
		if (base.GlobalPosition.DistanceTo(playerObject.GlobalPosition) < 5f && aggro)
		{
			anger += (float)delta;
		}
		if (anger > 1f)
		{
			anger = 0f;
			EnemyAI.Set("attacking", false);
			AnimTrigger("Attack");
			aggro = false;
		}
	}

	public void animEnd(StringName name)
	{
		GD.Print(name.ToString());
		if (name.ToString() == "RingAttack")
		{
			AnimTrigger("walking");
			EnemyAI.Set("attacking", true);
			aggro = true;
		}
	}

	public async void AnimTrigger(string triggerName)
	{
		GD.Print("Triggering animation " + triggerName);
		anim.Set("parameters/conditions/" + triggerName, true);
		await Task.Delay(TimeSpan.FromSeconds(0.1));
		if (GodotObject.IsInstanceValid(anim))
		{
			anim.Set("parameters/conditions/" + triggerName, false);
		}
	}
}
