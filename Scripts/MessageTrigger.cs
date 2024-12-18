using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/MessageTrigger.cs")]
public partial class MessageTrigger : Area3D
{
	[Export(PropertyHint.None, "")]
	public bool hideMessage;

	private DialogueBox DialogueBox;

	[Export(PropertyHint.None, "")]
	public string name;

	private bool used;

	[Export(PropertyHint.MultilineText, "")]
	public string Message;

	[Export(PropertyHint.None, "")]
	public Texture2D faceSprite;

	public override void _Ready()
	{
		base.AreaEntered += OnAreaEntered;
		DialogueBox = DialogueBox.instance;
	}

	private void OnAreaEntered(Node3D playerNode)
	{
		if (!used)
		{
			if (hideMessage)
			{
				GD.Print("Entered a Hide Message node!");
				DialogueBox.instance.Hide();
			}
			else
			{
				GD.Print("Entered a Message node!");
				used = true;
				ShowMessage();
			}
		}
	}

	public async void ShowMessage()
	{
		DialogueBox.GetNode<Label>("NameText").Text = name;
		DialogueBox.GetNode<RichTextLabel>("MainText").Text = Message;
		DialogueBox.GetNode<Sprite2D>("FaceSprite").Texture = faceSprite;
		DialogueBox.Show();
		DialogueBox.Modulate = Colors.Transparent;
		Tween tween = CreateTween();
		tween.TweenProperty(DialogueBox, "modulate", Colors.White, 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		await Task.Delay(TimeSpan.FromSeconds(2.0));
		QueueFree();
	}
}
