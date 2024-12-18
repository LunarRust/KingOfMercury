using Godot;

[ScriptPath("res://Scripts/NumberButton.cs")]
public partial class NumberButton : Button
{
	[Export(PropertyHint.None, "")]
	public string number;

	public override void _Pressed()
	{
		Phone.instance.changeNumber(number);
	}
}
