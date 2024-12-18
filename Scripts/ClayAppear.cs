using Godot;

[ScriptPath("res://Scripts/ClayAppear.cs")]
public partial class ClayAppear : Area3D
{
	[Export(PropertyHint.None, "")]
	public Node3D clayPerson;

	public override void _Ready()
	{
		clayPerson.ProcessMode = ProcessModeEnum.Disabled;
		clayPerson.Hide();
	}

	public void OnAreaEntered(Area3D area)
	{
		clayPerson.ProcessMode = ProcessModeEnum.Inherit;
		clayPerson.Show();
	}
}
