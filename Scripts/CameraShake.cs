using Godot;

[ScriptPath("res://Scripts/CameraShake.cs")]
public partial class CameraShake : Node3D
{
	[Export(PropertyHint.None, "")]
	private float shakeDuration = 0.1f;

	[Export(PropertyHint.None, "")]
	private float shakeAmount = 0.2f;

	[Export(PropertyHint.None, "")]
	public Camera3D cam;

	public static float shakeStrength = 0f;

	public static float shakeDecay = 1f;

	private RandomNumberGenerator rng;

	public override void _Ready()
	{
		shakeStrength = 0f;
		rng = new RandomNumberGenerator();
		base._Ready();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (shakeStrength > 0f)
		{
			cam.Rotation = new Vector3(RandomOffset().Y, RandomOffset().X, 0f);
			shakeStrength -= (float)delta * shakeDecay;
		}
	}

	public Vector2 RandomOffset()
	{
		return new Vector2(rng.RandfRange(0f - shakeStrength, shakeStrength), rng.RandfRange(0f - shakeStrength, shakeStrength));
	}

	public static void Shake(float power, float decay = 1f)
	{
		Input.StartJoyVibration(0, 0.5f, 0.5f, 0.25f);
		shakeDecay = decay;
		shakeStrength = power;
	}

	public void shakeNew(double delta, float power = 1f, float length = 1f)
	{
		Camera3D camera3D = GetViewport().GetCamera3D();
		Vector3 rotation = camera3D.Rotation;
		float num = length;
		float num2 = power;
		while (num > 0f)
		{
			num -= (float)delta * 1f;
			num2 -= (float)delta * 1f;
			Vector2 vector = new Vector2(rng.RandfRange(0f - num2, num2), rng.RandfRange(0f - num2, num2));
			camera3D.Rotation = rotation + new Vector3(vector.Y, vector.X, 0f);
		}
		camera3D.Rotation = rotation;
	}
}
