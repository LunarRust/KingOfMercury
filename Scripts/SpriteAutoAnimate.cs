using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/SpriteAutoAnimate.cs")]
public partial class SpriteAutoAnimate : Sprite2D
{
	private int frameCount;

	private int increase = 1;

	[Export(PropertyHint.None, "")]
	public float frameInterval = 0.1f;

	[Export(PropertyHint.None, "")]
	public bool PingPong;

	public override void _Ready()
	{
		frameCount = base.Hframes * base.Vframes;
		GD.Print(frameCount);
		animate();
	}

	public async void animate()
	{
		if (PingPong)
		{
			await Task.Delay(TimeSpan.FromSeconds(frameInterval));
			if (base.Frame == frameCount - 1)
			{
				increase = -1;
			}
			else if (base.Frame == 0)
			{
				increase = 1;
			}
			base.Frame += increase;
			animate();
		}
		else
		{
			await Task.Delay(TimeSpan.FromSeconds(frameInterval));
			if (base.Frame == frameCount - 1)
			{
				base.Frame = 0;
			}
			else
			{
				base.Frame++;
			}
			animate();
		}
	}
}
