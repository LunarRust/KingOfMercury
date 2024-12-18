using Godot;

[ScriptPath("res://Scripts/DNABehavior.cs")]
public partial class DNABehavior : Node3D
{
	public static int int1;

	public static int int2;

	public static int int3;

	public static int int4;

	[Export(PropertyHint.None, "")]
	public Sprite2D baseSprite;

	[Export(PropertyHint.None, "")]
	public Sprite2D digitSprite1;

	[Export(PropertyHint.None, "")]
	public Sprite2D digitSprite2;

	[Export(PropertyHint.None, "")]
	public Sprite2D digitSprite3;

	[Export(PropertyHint.None, "")]
	public Sprite2D digitSprite4;

	[Export(PropertyHint.None, "")]
	public RichTextLabel lockText;

	[Export(PropertyHint.None, "")]
	public Sprite2D lockSprite1;

	[Export(PropertyHint.None, "")]
	public Sprite2D lockSprite2;

	[Export(PropertyHint.None, "")]
	public Sprite2D lockSprite3;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer soundSource;

	[Export(PropertyHint.None, "")]
	public AudioStream changeSound;

	[Export(PropertyHint.None, "")]
	public AudioStream failSound;

	[Export(PropertyHint.None, "")]
	public AudioStream trySound;

	[Export(PropertyHint.None, "")]
	public DialogueSystem alleleDialogue;

	[Export(PropertyHint.None, "")]
	private Sprite2D cursor;

	private Node3D playerObject;

	private bool puzzleFinished;

	public static bool lock1 = true;

	public static bool lock2 = true;

	public static bool lock3 = true;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer anim;

	public override void _Ready()
	{
		cursor.ProcessMode = ProcessModeEnum.Disabled;
		playerObject = GetTree().GetFirstNodeInGroup("player") as Node3D;
	}

	public override void _Process(double delta)
	{
		if (baseSprite.Visible && base.GlobalPosition.DistanceTo(playerObject.GlobalPosition) > 2f)
		{
			Close();
		}
	}

	public void Touch()
	{
		hudmanager.HideHUD();
		baseSprite.Show();
		playerObject.ProcessMode = ProcessModeEnum.Disabled;
		cursor.ProcessMode = ProcessModeEnum.Inherit;
		if (!lock1 && !lock2 && !lock3)
		{
			lockText.Text = "[center]Injector Unlocked!";
		}
		if (!lock1)
		{
			lockSprite1.Frame = 1;
		}
		if (!lock2)
		{
			lockSprite2.Frame = 1;
		}
		if (!lock3)
		{
			lockSprite3.Frame = 1;
		}
	}

	public void KillButton()
	{
		if (!lock1 && !lock2 && !lock3)
		{
			soundSource.Stream = trySound;
			soundSource.Play();
			DNACheck();
		}
		else
		{
			soundSource.Stream = failSound;
			soundSource.Play();
			GD.Print("Locks Active!");
		}
	}

	public void dnaChange(Sprite2D display, int intInstance)
	{
		intInstance = ((intInstance >= 4) ? 1 : (intInstance + 1));
		display.Frame = intInstance - 1;
	}

	public void DNAButton(int digit)
	{
		soundSource.Stream = changeSound;
		soundSource.Play();
		if (digit == 1)
		{
			if (int1 < 4)
			{
				int1++;
			}
			else
			{
				int1 = 1;
			}
			digitSprite1.Frame = int1 - 1;
		}
		if (digit == 2)
		{
			if (int2 < 4)
			{
				int2++;
			}
			else
			{
				int2 = 1;
			}
			digitSprite2.Frame = int2 - 1;
		}
		if (digit == 3)
		{
			if (int3 < 4)
			{
				int3++;
			}
			else
			{
				int3 = 1;
			}
			digitSprite3.Frame = int3 - 1;
		}
		if (digit == 4)
		{
			if (int4 < 4)
			{
				int4++;
			}
			else
			{
				int4 = 1;
			}
			digitSprite4.Frame = int4 - 1;
		}
	}

	private void DNACheck()
	{
		string text = int1.ToString() + int2 + int3 + int4;
		GD.Print(text);
		switch (text)
		{
		default:
			if (!(text == "1342"))
			{
				if (!puzzleFinished)
				{
					Close();
					anim.Play("Attempt");
				}
				break;
			}
			goto case "1432";
		case "1432":
		case "2341":
		case "2431":
			if (!puzzleFinished)
			{
				Close();
				DNAKill();
			}
			break;
		}
	}

	public void Close()
	{
		baseSprite.Hide();
		hudmanager.ShowHUD();
		cursor.ProcessMode = ProcessModeEnum.Disabled;
		playerObject.ProcessMode = ProcessModeEnum.Inherit;
	}

	private void DNAKill()
	{
		puzzleFinished = true;
		alleleDialogue.Dialogue = "Ah, nice work on the malignant DNA. Good thing that's over. You made sure that the injection was local, right?";
		anim.Play("Kill");
	}
}
