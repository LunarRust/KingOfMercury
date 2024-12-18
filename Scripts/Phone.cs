using Godot;

[ScriptPath("res://Scripts/Phone.cs")]
public partial class Phone : Button
{
	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	public static Phone instance;

	[Export(PropertyHint.None, "")]
	public PhoneNumber[] validNumbers;

	public string PhoneNumber = "";

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer sfxSource;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer CallSource;

	[Export(PropertyHint.None, "")]
	public AudioStreamOggVorbis keySound;

	[Export(PropertyHint.None, "")]
	public AudioStreamOggVorbis failSound;

	[Export(PropertyHint.None, "")]
	public Label numberLabel;

	private GDScript ACH = (GDScript)GD.Load("res://Scripts/SteamAchievement.gd");

	public override void _Ready()
	{
		instance = this;
	}

	public void changeNumber(string number)
	{
		if (PhoneNumber.Length < 4)
		{
			sfxSource.Stream = keySound;
			sfxSource.Play();
			PhoneNumber += number;
			numberLabel.Text = PhoneNumber;
		}
		else
		{
			sfxSource.Stream = failSound;
			sfxSource.Play();
			GD.Print("No More Room for Numbers");
		}
	}

	public override void _Pressed()
	{
		PhoneNumber[] array = validNumbers;
		foreach (PhoneNumber phoneNumber in array)
		{
			if (phoneNumber.Number == PhoneNumber)
			{
				ACH.Call("_static_grant", "ACH_PHONE");
				anim.Stop();
				anim.Play(phoneNumber.AudioPath);
			}
		}
		PhoneNumber = "";
		numberLabel.Text = PhoneNumber;
	}
}
