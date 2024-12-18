using Godot;

[ScriptPath("res://Scripts/lookatplayer.cs")]
public partial class lookatplayer : Node3D
{
	[Export(PropertyHint.None, "")]
	public Node3D playerNode;

	private Vector3 lookTarget;

	private RandomNumberGenerator rng;

	[Export(PropertyHint.None, "")]
	public float shakeMagnitude = 1f;

	public override void _Ready()
	{
		rng = new RandomNumberGenerator();
		if (playerNode != null)
		{
		}
	}

	public override void _Process(double delta)
	{
		lookTarget = playerNode.GlobalPosition + new Vector3(rng.RandfRange(0f - shakeMagnitude, shakeMagnitude), rng.RandfRange(0f - shakeMagnitude, shakeMagnitude), rng.RandfRange(0f - shakeMagnitude, shakeMagnitude));
		LookAt(lookTarget);
	}
}
