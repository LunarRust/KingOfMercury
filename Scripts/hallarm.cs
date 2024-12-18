using Godot;
using Godot.Collections;

[ScriptPath("res://Scripts/hallarm.cs")]
public partial class hallarm : Area3D
{
	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	public override void _Ready()
	{
		anim = GetNode<AnimationPlayer>("../AnimationPlayer");
		anim.Stop();
		RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
		anim.Seek(randomNumberGenerator.RandfRange(0f, 3f), update: true);
		anim.Play();
	}

	public void DamageCheck()
	{
		Array<Area3D> overlappingAreas = GetOverlappingAreas();
		foreach (Area3D item in overlappingAreas)
		{
			if (item.Name == (StringName)"PlayerArea")
			{
				PlayerHealthHandler.instance.changeHealth(-3);
				AnimationPlayer animationPlayer = new AnimationPlayer();
			}
		}
	}
}
