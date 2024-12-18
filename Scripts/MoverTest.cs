using System.Collections;
using Godot;
using Godot.Collections;

[ScriptPath("res://Scripts/MoverTest.cs")]
public partial class MoverTest : Node3D
{
	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	public AudioStreamOggVorbis stepSound;

	[Export(PropertyHint.None, "")]
	public ShapeCast3D shapeCaster;

	[Export(PropertyHint.None, "")]
	public Node3D castBase;

	[Export(PropertyHint.None, "")]
	public Area3D areaCast;

	public Node3D moveBasis;

	public Vector3 newPos;

	public Vector3 newRot;

	public bool canMove;

	public bool canTurn;

	private Sprite2D UISprite;

	private float speed = 5f;

	public static bool running;

	[Export(PropertyHint.None, "")]
	public Camera3D camera;

	[Export(PropertyHint.None, "")]
	public Node3D head;

	private bool camFree;

	public static bool camSet;

	private Vector2 lookAxis;

	[Export(PropertyHint.None, "")]
	public AnimationTree playerAnim;

	public Vector2 animCurrentSpeed;

	public Vector2 animTargetSpeed;

	private float headTurn = 0.02f;

	private float headTilt = -0.1f;

	public static MoverTest instance;

	public static Camera3D camInstance;

	public override void _Ready()
	{
		instance = this;
		camInstance = camera;
		setPos();
		base.Position = base.Position.Snapped(new Vector3(0.5f, 0.5f, 0.5f));
		newPos = base.Position;
		newRot = base.Rotation;
		UISprite = GetNode<Sprite2D>("%UI");
		speed = 5f;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	public override void _Process(double delta)
	{
		lookAxis = new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));
		if (lookAxis.Length() < 0.1f)
		{
			lookAxis = Vector2.Zero;
		}
		animCurrentSpeed = animCurrentSpeed.Lerp(animTargetSpeed, (float)delta * 10f);
		if (playerAnim != null)
		{
			playerAnim.Set("parameters/Normal2D/blend_position", animCurrentSpeed);
		}
		move((float)delta);
		base.Position = base.Position.Lerp(newPos, (float)delta * speed);
		base.Rotation = base.Rotation.Lerp(newRot, (float)delta * 5f);
		if (base.Position.DistanceTo(newPos) < 0.25f && !canMove)
		{
			if (!Input.IsActionPressed("Up"))
			{
				animTargetSpeed.X = 0f;
				animTargetSpeed.Y = 0f;
			}
			canMove = true;
		}
		if (base.Rotation.DistanceTo(newRot) < 0.25f && !canTurn)
		{
			animTargetSpeed.X = 0f;
			animTargetSpeed.Y = 0f;
			canTurn = true;
		}
		if ((Input.IsActionPressed("Look") || (lookAxis != Vector2.Zero && !InventoryButton.open)) && !Input.IsActionPressed("Run"))
		{
			camFree = true;
		}
		if (!Input.IsActionPressed("Look") && lookAxis == Vector2.Zero && camFree)
		{
			camFree = false;
			Tween tween = CreateTween();
			tween.SetParallel();
			tween.TweenProperty(camera, "rotation", Vector3.Zero, 0.5).SetTrans(Tween.TransitionType.Quad);
			tween.TweenProperty(head, "rotation", Vector3.Zero, 0.5).SetTrans(Tween.TransitionType.Quad);
		}
		if (castBase != null)
		{
			castBase.GlobalPosition = newPos;
			castBase.GlobalRotation = newRot;
		}
	}

	public void CameraReset()
	{
		camera.MakeCurrent();
		camera.GetNode<AudioListener3D>("AudioListener3D").MakeCurrent();
		hudmanager.ShowHUD();
	}

	public void setPos()
	{
		if (SceneTransfer.setPosition)
		{
			GD.Print("Setting Position to " + SceneTransfer.endPosition);
			base.Position = SceneTransfer.endPosition;
			newPos = SceneTransfer.endPosition;
			SceneTransfer.setPosition = false;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (camFree)
		{
			if (@event is InputEventMouseMotion inputEventMouseMotion)
			{
				head.RotateY(Mathf.DegToRad((0f - inputEventMouseMotion.Relative.X) * 0.25f));
				camera.RotateX(Mathf.DegToRad((0f - inputEventMouseMotion.Relative.Y) * 0.25f));
			}
			else if (lookAxis != Vector2.Zero)
			{
				head.Rotation = new Vector3(0f, 0f - lookAxis.X, 0f);
				camera.Rotation = new Vector3(0f - lookAxis.Y, 0f, 0f);
			}
		}
	}

	public void move(float delta)
	{
		if (Input.IsActionPressed("Run") && PlayerHealthHandler.mana > 1)
		{
			speed = 6.5f;
			running = true;
		}
		else if (running)
		{
			Tween tween = CreateTween();
			tween.TweenProperty(camInstance, "rotation", Vector3.Zero, 0.20000000298023224);
			speed = 5f;
			running = false;
		}
		if (Input.IsActionPressed("Up") && canMove && canTurn && !PlayerHealthHandler.dead)
		{
			animTargetSpeed.X = 0f;
			animTargetSpeed.Y = 1f;
			if (castBase != null)
			{
				hitTest(1f, -castBase.GlobalBasis.Z);
			}
			else
			{
				hitTest(1f, -base.Transform.Basis.Z);
			}
			headTilt = -0.1f;
		}
		if (Input.IsActionPressed("Down") && canMove && canTurn && !PlayerHealthHandler.dead)
		{
			animTargetSpeed.X = 0f;
			animTargetSpeed.Y = -1f;
			if (castBase != null)
			{
				hitTest(1f, castBase.GlobalBasis.Z, useStamina: true, 2);
			}
			else
			{
				hitTest(1f, base.Transform.Basis.Z, useStamina: true, 2);
			}
			headTilt = 0.1f;
		}
		if (Input.IsActionPressed("StrafeLeft") && canMove && canTurn && !PlayerHealthHandler.dead)
		{
			animTargetSpeed.X = 0f;
			animTargetSpeed.Y = 1f;
			if (castBase != null)
			{
				hitTest(1f, -castBase.GlobalBasis.X, useStamina: false);
			}
			else
			{
				hitTest(1f, -base.Transform.Basis.X, useStamina: false);
			}
		}
		if (Input.IsActionPressed("StrafeRight") && canMove && canTurn && !PlayerHealthHandler.dead)
		{
			animTargetSpeed.X = 0f;
			animTargetSpeed.Y = 1f;
			if (castBase != null)
			{
				hitTest(1f, castBase.GlobalBasis.X, useStamina: false);
			}
			else
			{
				hitTest(1f, base.Transform.Basis.X, useStamina: false);
			}
		}
		if (Input.IsActionPressed("Left") && canTurn && !PlayerHealthHandler.dead)
		{
			animTargetSpeed.X = -1f;
			animTargetSpeed.Y = 0f;
			Turn(1f);
		}
		if (Input.IsActionPressed("Right") && canTurn && !PlayerHealthHandler.dead)
		{
			animTargetSpeed.X = 1f;
			animTargetSpeed.Y = 0f;
			Turn(-1f);
		}
	}

	public IEnumerator moving()
	{
		yield return null;
	}

	public void hitTest(float amount, Vector3 direction, bool useStamina = true, int manaAmount = 1)
	{
		PhysicsDirectSpaceState3D directSpaceState = GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D physicsRayQueryParameters3D = PhysicsRayQueryParameters3D.Create(newPos - base.Transform.Basis.Y * 0.1f, newPos + direction * 1.1f);
		physicsRayQueryParameters3D.HitBackFaces = false;
		Dictionary dictionary = directSpaceState.IntersectRay(physicsRayQueryParameters3D);
		if (dictionary.Count > 0)
		{
			animTargetSpeed.X = 0f;
			animTargetSpeed.Y = 0f;
			return;
		}
		directSpaceState = GetWorld3D().DirectSpaceState;
		Vector3 vector = newPos + direction * amount;
		PhysicsRayQueryParameters3D physicsRayQueryParameters3D2 = PhysicsRayQueryParameters3D.Create(vector, vector + -base.Transform.Basis.Y * 2f);
		physicsRayQueryParameters3D2.CollisionMask = 1u;
		Dictionary dictionary2 = directSpaceState.IntersectRay(physicsRayQueryParameters3D2);
		if (dictionary2.Count > 0)
		{
			RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
			soundSource.PitchScale = randomNumberGenerator.RandfRange(0.8f, 1.2f);
			soundSource.Play();
			canMove = false;
			Vector3 vector2 = newPos + direction * amount;
			newPos = new Vector3(vector2.X, dictionary2["position"].AsVector3().Y + 1f, vector2.Z);
			newPos = newPos.Snapped(new Vector3(0.5f, 0f, 0.5f));
			if (running && useStamina)
			{
				headTurn = 0f - headTurn;
				Tween tween = CreateTween();
				tween.TweenProperty(camInstance, "rotation", new Vector3(headTilt, 0f, headTurn), 0.20000000298023224).SetTrans(Tween.TransitionType.Quad);
				PlayerHealthHandler.mana -= manaAmount;
			}
			Tween tween2 = CreateTween();
			UISprite.Position = Vector2.Zero;
			tween2.TweenProperty(UISprite, "position", Vector2.Down * 10f, 0.20000000298023224).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
			tween2.TweenProperty(UISprite, "position", Vector2.Zero * 10f, 0.10000000149011612).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		}
		else
		{
			GD.Print("No floor detected, cannot move.");
			animTargetSpeed.X = 0f;
			animTargetSpeed.Y = 0f;
		}
	}

	public void Turn(float amount)
	{
		newRot += new Vector3(0f, 1.5708f * amount, 0f);
		canTurn = false;
	}

	public static void StaticTurn(int amount)
	{
		instance.newRot += new Vector3(0f, 1.5708f * (float)amount, 0f);
		instance.canTurn = false;
	}
}
