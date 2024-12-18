using Godot;

[ScriptPath("res://Scenes/LockDoor.cs")]
public partial class LockDoor : Node
{
	[Export(PropertyHint.None, "")]
	public string itemMatch;

	public bool opened;

	public override void _Ready()
	{
	}

	public bool Item(string item)
	{
		GD.Print("Trying Key");
		GD.Print("Does " + item + " equal " + itemMatch);
		if (item == itemMatch && !opened)
		{
			Open();
			return true;
		}
		return false;
	}

	public void Open()
	{
		if (!opened)
		{
			opened = true;
			AudioStreamPlayer node = GetNode<AudioStreamPlayer>("%SoundSource");
			node.Stream = GD.Load("res://Sounds/DoorBig.ogg") as AudioStream;
			node.Play();
			Tween tween = CreateTween().SetParallel();
			tween.TweenProperty(GetParent(), "position", Vector3.Up * 4f, 2.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine)
				.AsRelative();
			tween.Finished += delegate
			{
				GetParent().QueueFree();
			};
		}
	}

	public void Destroy()
	{
		QueueFree();
	}
}
