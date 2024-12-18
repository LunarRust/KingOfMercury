using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/HitStop.cs")]
public partial class HitStop : Node
{
	public static async void Stop(float timescale, float duration)
	{
		Engine.TimeScale = timescale;
		await Task.Delay(TimeSpan.FromSeconds(duration));
		Engine.TimeScale = 1.0;
	}
}
