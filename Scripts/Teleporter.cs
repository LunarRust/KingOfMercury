using Godot;

[ScriptPath("res://Scripts/Teleporter.cs")]
public partial class Teleporter : Area3D
{
	[Export(PropertyHint.None, "")]
	public Vector3 destination;

	[Export(PropertyHint.None, "")]
	public ColorRect colorRect;

	public MoverTest playerObject;

	public override void _Ready()
	{
		playerObject = MoverTest.instance;
		base.AreaEntered += teleport;
	}

	public void teleport(Area3D area)
	{
		if (area.Name == (StringName)"PlayerArea")
		{
			colorRect.Color = Colors.White;
			Tween tween = CreateTween();
			tween.TweenProperty(colorRect, "color", Colors.Transparent, 4.0);
			playerObject.Position = destination + Vector3.Up;
			playerObject.newPos = destination + Vector3.Up;
		}
	}
}
