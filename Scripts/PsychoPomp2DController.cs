using Godot;

[ScriptPath("res://Scripts/PsychoPomp2DController.cs")]
public partial class PsychoPomp2DController : CharacterBody3D
{
	[Export(PropertyHint.None, "")]
	public float speed = 2f;

	private int speedModifier = 1;

	[Export(PropertyHint.None, "")]
	public Sprite3D charSprite;

	[Export(PropertyHint.None, "")]
	public Texture2D sprite1;

	[Export(PropertyHint.None, "")]
	public Texture2D sprite2;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	private Tween tween;

	private Vector2 input;

	private Vector3 camPosition;

	[Export(PropertyHint.None, "")]
	public Camera3D cam;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		GetInput();
		if ((double)input.X < -0.1)
		{
			charSprite.FlipH = true;
		}
		else if ((double)input.X > 0.1)
		{
			charSprite.FlipH = false;
		}
		if ((double)input.Y < -0.1)
		{
			charSprite.Texture = sprite1;
		}
		else if ((double)input.Y > 0.1)
		{
			charSprite.Texture = sprite2;
		}
		base.Velocity = new Vector3(input.X, base.Velocity.Y - 10f * (float)delta, input.Y);
		MoveAndSlide();
		if (input.Length() != 0f && anim.CurrentAnimation != "Walk")
		{
			anim.Play("Walk");
		}
		else if (input.Length() == 0f)
		{
			anim.Play("RESET");
		}
		anim.SpeedScale = speedModifier;
	}

	public void GetInput()
	{
		input.X = Input.GetAxis("Left", "Right");
		input.Y = Input.GetAxis("Up", "Down");
		if (Input.IsActionPressed("Run"))
		{
			speedModifier = 2;
		}
		else
		{
			speedModifier = 1;
		}
		speed = 3 * speedModifier;
		input = input.Normalized() * speed;
	}
}
