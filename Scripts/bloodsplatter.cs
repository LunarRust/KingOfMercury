using Godot;
using Godot.Collections;

[ScriptPath("res://Scripts/bloodsplatter.cs")]
public partial class bloodsplatter : GpuParticles3D
{
	[Export(PropertyHint.None, "")]
	public Node3D bloodMark;

	public override void _Ready()
	{
		bloodCheck();
		Restart();
	}

	public async void bloodCheck()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		if (bloodMark == null)
		{
			return;
		}
		PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
		Vector3 Vpos = base.Position;
		PhysicsRayQueryParameters3D queryV = PhysicsRayQueryParameters3D.Create(Vpos, Vpos + Vector3.Down * 2f);
		Dictionary resultV = spaceState.IntersectRay(queryV);
		if (resultV.Count > 0)
		{
			bloodMark.GlobalPosition = (Vector3)resultV["position"] + Vector3.Up * 0.01f;
			Vector3 targetRotation = (Vector3)resultV["normal"];
			if (!(targetRotation != Vector3.Up))
			{
			}
		}
		else
		{
			GD.Print("No Point for Blood Found!");
			bloodMark.QueueFree();
		}
	}
}
