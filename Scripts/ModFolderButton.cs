using Godot;

[ScriptPath("res://Scripts/ModFolderButton.cs")]
public partial class ModFolderButton : TextureButton
{
	public override void _Ready()
	{
	}

	public override void _Pressed()
	{
		GD.Print("Attempting to open user folder");
		OS.ShellOpen(ProjectSettings.GlobalizePath("user://mods/"));
	}
}
