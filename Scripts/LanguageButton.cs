using Godot;

[ScriptPath("res://Scripts/LanguageButton.cs")]
public partial class LanguageButton : TextureButton
{
	[Export(PropertyHint.None, "")]
	public string[] languages;

	private int currentLanguage;

	public override void _Ready()
	{
		string[] array = languages;
		foreach (string text in array)
		{
			if (text == TranslationServer.GetLocale().ToString())
			{
				GD.Print("currentlanguage value is " + currentLanguage);
			}
		}
	}

	public override void _Pressed()
	{
		if (currentLanguage < languages.Length - 1)
		{
			currentLanguage++;
			TranslationServer.SetLocale(languages[currentLanguage]);
			base._Pressed();
		}
		else
		{
			currentLanguage = 0;
			TranslationServer.SetLocale(languages[currentLanguage]);
		}
	}
}
