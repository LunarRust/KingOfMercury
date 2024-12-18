using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using MEC;

[ScriptPath("res://Scripts/CameraCast.cs")]
public partial class CameraCast : Camera3D
{
	[Signal]
	public delegate void TouchEventHandler();

	public static Camera3D instance;

	public static CameraCast codeInstance;

	public Vector2 mousePosition;

	[Export(PropertyHint.None, "")]
	public bool canAttack;

	[Export(PropertyHint.None, "")]
	public AnimationTree playerAnim;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer soundSource;

	public static bool usedHammer;

	public override void _Ready()
	{
		usedHammer = false;
		instance = this;
		codeInstance = this;
		canAttack = true;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left && inputEventMouseButton.Pressed && !InventoryButton.open)
		{
			mousePosition = inputEventMouseButton.Position;
			if (InteractionButton.interactionMode != 4)
			{
				Cast();
			}
			else if (InteractionButton.interactionMode == 4 && canAttack)
			{
				Timing.RunCoroutine(Attack());
			}
		}
		else if (Input.IsActionJustPressed("ControllerAction") && !InventoryButton.open)
		{
			mousePosition = new Vector2(480f, 270f);
			if (InteractionButton.interactionMode != 4)
			{
				Cast();
			}
			else if (InteractionButton.interactionMode == 4 && canAttack)
			{
				Timing.RunCoroutine(Attack());
			}
		}
		base._Input(@event);
	}

	public void Cast()
	{
		float zDepth = ((!ViewButton.thirdActive) ? 2f : 4f);
		PhysicsDirectSpaceState3D directSpaceState = GetWorld3D().DirectSpaceState;
		Vector3 from = ProjectRayOrigin(mousePosition);
		Vector3 to = ProjectPosition(mousePosition, zDepth);
		PhysicsRayQueryParameters3D physicsRayQueryParameters3D = PhysicsRayQueryParameters3D.Create(from, to);
		physicsRayQueryParameters3D.HitBackFaces = false;
		Dictionary dictionary = directSpaceState.IntersectRay(physicsRayQueryParameters3D);
		GD.Print(dictionary);
		if (dictionary.Count > 0 && InteractionButton.interactionMode == 4)
		{
			soundSource.Stream = GD.Load("res://Sounds/Impact.ogg") as AudioStream;
			soundSource.Play();
		}
		if (dictionary.Count > 0 && dictionary["collider"].As<CollisionObject3D>().HasNode("DialogueSystem"))
		{
			dictionary["collider"].As<CollisionObject3D>().GetNode<DialogueSystem>("DialogueSystem").DialogueProcessing();
		}
		if (dictionary.Count > 0 && dictionary["collider"].As<CollisionObject3D>().HasNode("Behavior"))
		{
			Node node = dictionary["collider"].As<CollisionObject3D>().GetNode("Behavior");
			if (InteractionButton.interactionMode == 1 && node.HasMethod("Look"))
			{
				node.Call("Look");
			}
			if (InteractionButton.interactionMode == 2 && node.HasMethod("Talk"))
			{
				if (playerAnim != null)
				{
					AnimTrigger("Talk");
				}
				node.Call("Talk");
			}
			if (InteractionButton.interactionMode == 3 && node.HasMethod("Touch"))
			{
				if (playerAnim != null)
				{
					AnimTrigger("Touch");
				}
				node.CallDeferred("Touch");
			}
			if (InteractionButton.interactionMode == 4 && node.HasMethod("Hurt"))
			{
				node.Call("Hurt");
			}
		}
		if (dictionary.Count > 0 && InteractionButton.interactionMode == 4 && dictionary["collider"].As<CollisionObject3D>().HasNode("HealthHandler"))
		{
			dictionary["collider"].As<CollisionObject3D>().GetNode<EnemyHealthHandler>("HealthHandler").Hurt(1);
		}
	}

	public IEnumerator<double> Attack()
	{
		usedHammer = true;
		if (playerAnim != null)
		{
			AnimTrigger("Attack");
		}
		soundSource.Stream = GD.Load("res://Sounds/Woosh.ogg") as AudioStream;
		soundSource.Play();
		canAttack = false;
		anim.Play("Attack");
		yield return Timing.WaitForSeconds(0.15000000596046448);
		Cast();
		yield return Timing.WaitForSeconds(1.0);
		canAttack = true;
	}

	public bool ItemCast(string item)
	{
		PhysicsDirectSpaceState3D directSpaceState = GetWorld3D().DirectSpaceState;
		Vector3 from = ProjectRayOrigin(mousePosition);
		Vector3 to = ProjectPosition(mousePosition, 2f);
		PhysicsRayQueryParameters3D parameters = PhysicsRayQueryParameters3D.Create(from, to);
		Dictionary dictionary = directSpaceState.IntersectRay(parameters);
		GD.Print(dictionary);
		GD.Print("Casting with " + item);
		if (dictionary.Count > 0 && dictionary["collider"].As<CollisionObject3D>().HasNode("Behavior"))
		{
			Node node = dictionary["collider"].As<CollisionObject3D>().GetNode("Behavior");
			if (node.HasMethod("Item"))
			{
				if ((bool)node.Call("Item", item))
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	public async void AnimTrigger(string triggerName)
	{
		playerAnim.Set("parameters/conditions/" + triggerName, true);
		await Task.Delay(TimeSpan.FromSeconds(0.1));
		if (GodotObject.IsInstanceValid(playerAnim))
		{
			playerAnim.Set("parameters/conditions/" + triggerName, false);
		}
	}
}
