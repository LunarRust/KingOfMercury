using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/ThraitBehavior.cs")]
public partial class ThraitBehavior : Node
{
	[Export(PropertyHint.None, "")]
	public AnimationTree anim;

	[Export(PropertyHint.None, "")]
	public Node3D EnemyAI;

	[Export(PropertyHint.None, "")]
	private DialogueSystem dialogue;

	public override void _Ready()
	{
		anim.AnimationFinished += startAttack;
	}

	public void startAttack(StringName name)
	{
		GD.Print(name.ToString());
		if (name.ToString() == "Angry")
		{
			EnemyAI.Set("attacking", true);
		}
	}

	public override void _Process(double delta)
	{
		if (InteractionButton.interactionMode != 1)
		{
		}
	}

	public void Touch()
	{
		AnimTrigger("Touch");
	}

	public void Talk()
	{
		AnimTrigger("Talk");
	}

	public void Hurt()
	{
		DialogueBox.instance.Hide();
		if (dialogue != null)
		{
			dialogue.looking = false;
		}
		anim.Set("parameters/conditions/Hurt", true);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton { Pressed: not false } && InteractionButton.interactionMode == 4 && CameraCast.codeInstance.canAttack)
		{
			AnimTrigger("Scared");
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
