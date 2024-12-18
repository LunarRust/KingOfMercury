using Godot;

[ScriptPath("res://Scripts/TipLoader.cs")]
public partial class TipLoader : RichTextLabel
{
	[Export(PropertyHint.None, "")]
	public string preface;

	[Export(PropertyHint.MultilineText, "")]
	public string[] tips;

	private RichTextLabel textLabel;

	public override void _Ready()
	{
		RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
		int num = randomNumberGenerator.RandiRange(0, tips.Length - 1);
		textLabel = this;
		textLabel.Text = "[wave]" + preface + "\n" + tips[num];
	}

	public override void _Process(double delta)
	{
	}
}
