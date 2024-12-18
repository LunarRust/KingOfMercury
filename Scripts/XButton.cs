using Godot;

[ScriptPath("res://Scripts/XButton.cs")]
public partial class XButton : TextureButton
{
	private Node2D bas;

	public override void _Ready()
	{
		bas = GetParent<Node2D>();
	}

	public override void _Process(double delta)
	{
	}

	public override void _Pressed()
	{
		bas.Hide();
		base._Pressed();
	}
}
