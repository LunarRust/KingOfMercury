using Godot;

[ScriptPath("res://Scripts/EyeTrigger.cs")]
public partial class EyeTrigger : Area3D
{
	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void _on_area_entered(Area3D area)
	{
		anim.Play("Open");
	}
}
