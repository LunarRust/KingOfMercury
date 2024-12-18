using Godot;

[GlobalClass]
[ScriptPath("res://Scripts/PhoneNumber.cs")]
public partial class PhoneNumber : Resource
{
	[Export(PropertyHint.None, "")]
	public string Number;

	[Export(PropertyHint.None, "")]
	public string AudioPath;
}
