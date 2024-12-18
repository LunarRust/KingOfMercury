using Godot;

[ScriptPath("res://Scripts/GateLever.cs")]
public partial class GateLever : Node
{
	[Export(PropertyHint.None, "")]
	public AnimationPlayer leverAnim;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer gateAnim;

	private bool used;

	public void Touch()
	{
		if (!used)
		{
			used = true;
			leverAnim.Play("TurnOn");
			gateAnim.Play("Open");
		}
	}
}
