using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/AnimeStalkBehavior.cs")]
public partial class AnimeStalkBehavior : Node
{
	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	public async void Touch()
	{
		PlayerHealthHandler.instance.changeHealth(-1);
		if (anim != null)
		{
			if (GodotObject.IsInstanceValid(anim))
			{
				anim.Play("Attack");
			}
			await Task.Delay(TimeSpan.FromSeconds(1.0));
			if (GodotObject.IsInstanceValid(anim))
			{
				anim.Play("Idle");
			}
		}
	}
}
