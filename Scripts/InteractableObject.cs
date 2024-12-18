using Godot;

[ScriptPath("res://Scripts/InteractableObject.cs")]
public partial class InteractableObject : Node
{
	[Export(PropertyHint.None, "")]
	public Node BehaviorNode;

	public void Interact(int mode)
	{
		if (mode == 1)
		{
			GD.Print("Trying to call 'Look' Method");
			if (BehaviorNode != null && BehaviorNode.HasMethod("Look"))
			{
				BehaviorNode.Call("Look");
			}
		}
		if (mode == 2 && BehaviorNode != null && BehaviorNode.HasMethod("Talk"))
		{
			BehaviorNode.Call("Talk");
		}
		if (mode == 3 && BehaviorNode != null && BehaviorNode.HasMethod("Touch"))
		{
			BehaviorNode.Call("Touch");
		}
		if (mode == 4)
		{
			if (BehaviorNode != null && BehaviorNode.HasMethod("Hurt"))
			{
				BehaviorNode.Call("Hurt");
			}
			if (GetParent().HasNode("HealthHandler"))
			{
				GetParent().GetNode<EnemyHealthHandler>("HealthHandler").Hurt(1);
			}
		}
	}
}
