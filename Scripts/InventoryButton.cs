using Godot;

[ScriptPath("res://Scripts/InventoryButton.cs")]
public partial class InventoryButton : TextureButton
{
	public static bool open;

	public static bool hidden;

	[Export(PropertyHint.None, "")]
	public Sprite2D hotbar;

	[Export(PropertyHint.None, "")]
	public Sprite2D gear1;

	[Export(PropertyHint.None, "")]
	public Sprite2D gear2;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer soundSource;

	public override void _Ready()
	{
		open = false;
	}

	public override void _Pressed()
	{
		if (!open)
		{
			Open();
		}
		else
		{
			Close();
		}
		base._Pressed();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("menu_inventory"))
		{
			if (!open)
			{
				Open();
			}
			else
			{
				Close();
			}
		}
		if (Input.IsActionPressed("MouseAction") && GetViewport().GetMousePosition().Y < 200f && open && !hidden)
		{
			Tween tween = CreateTween().SetParallel();
			tween.TweenProperty(hotbar, "position", new Vector2(480f, 475f), 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
			tween.TweenProperty(gear1, "rotation", 0f, 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
			tween.TweenProperty(gear2, "rotation", 0f, 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
			hidden = true;
		}
		else if (Input.IsActionJustReleased("MouseAction") && hidden)
		{
			Tween tween2 = CreateTween().SetParallel();
			tween2.TweenProperty(hotbar, "position", new Vector2(480f, 300f), 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
			tween2.TweenProperty(gear1, "rotation", 5f, 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
			tween2.TweenProperty(gear2, "rotation", -5f, 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
			hidden = false;
		}
	}

	public void Open()
	{
		open = true;
		soundSource.Stream = GD.Load("res://Sounds/InvOpen.ogg") as AudioStream;
		soundSource.Play();
		Tween tween = CreateTween().SetParallel();
		tween.TweenProperty(hotbar, "position", new Vector2(480f, 300f), 2.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(gear1, "rotation", 5f, 2.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(gear2, "rotation", -5f, 2.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
	}

	public void Close()
	{
		open = false;
		soundSource.Stream = GD.Load("res://Sounds/InvClose.ogg") as AudioStream;
		soundSource.Play();
		Tween tween = CreateTween().SetParallel();
		tween.TweenProperty(hotbar, "position", new Vector2(480f, 560f), 2.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(gear1, "rotation", 0f, 2.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(gear2, "rotation", 0f, 2.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
	}
}
