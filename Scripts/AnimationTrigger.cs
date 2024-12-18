using Godot;

[ScriptPath("res://Scripts/AnimationTrigger.cs")]
public partial class AnimationTrigger : Area3D
{
	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	[Export(PropertyHint.None, "")]
	public string animationName;

	[Export(PropertyHint.None, "")]
	public bool oneShot = true;

	[Export(PropertyHint.None, "")]
	public bool onExit;

	private bool played = false;

	public override void _Ready()
	{
		if (onExit)
		{
			base.AreaExited += PlayAnim;
		}
		else
		{
			base.AreaEntered += PlayAnim;
		}
	}

	public void PlayAnim(Area3D area)
	{
		if (!played)
		{
			anim.Play(animationName);
			if (oneShot)
			{
				played = true;
			}
		}
	}
}
