using Godot;

[ScriptPath("res://Scripts/DialogueSystem.cs")]
public partial class DialogueSystem : Node3D
{
	[Export(PropertyHint.None, "")]
	public string npcName;

	[Export(PropertyHint.MultilineText, "")]
	public string Dialogue;

	[Export(PropertyHint.MultilineText, "")]
	public string LookDescription;

	[Export(PropertyHint.MultilineText, "")]
	public string TouchDescription;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer3D soundSource;

	[Export(PropertyHint.None, "")]
	public AudioStream DialogueSound;

	[Export(PropertyHint.None, "")]
	public Texture2D faceSprite;

	private DialogueBox DialogueBox;

	private bool isTalking;

	private MoverTest PlayerObject;

	public Vector3 lookTarget;

	[Export(PropertyHint.None, "")]
	public bool looking = true;

	[Export(PropertyHint.None, "")]
	public bool Distance = true;

	private Node3D parentnode;

	public override void _Ready()
	{
		DialogueBox = DialogueBox.instance;
		PlayerObject = MoverTest.instance;
		parentnode = GetParent<Node3D>();
		Vector3 vector = parentnode.GlobalPosition + parentnode.Basis.Z * 2f;
		lookTarget = new Vector3(vector.X, base.GlobalPosition.Y, vector.Z);
	}

	public override void _Process(double delta)
	{
		if (looking)
		{
			parentnode.LookAt(lookTarget, Vector3.Up, useModelFront: true);
		}
		if (base.GlobalPosition.DistanceTo(PlayerObject.GlobalPosition) > 4f && isTalking && Distance)
		{
			CloseDialogue();
		}
	}

	public void OpenDialogue()
	{
		DialogueBox.Show();
		DialogueBox.Modulate = Colors.Transparent;
		Tween tween = CreateTween();
		tween.TweenProperty(DialogueBox, "modulate", Colors.White, 1.0).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
	}

	public void DialogueProcessing()
	{
		if (looking)
		{
			Tween tween = CreateTween();
			tween.TweenProperty(this, "lookTarget", new Vector3(MoverTest.instance.Position.X, base.GlobalPosition.Y, MoverTest.instance.Position.Z), 0.5).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		}
		if (InteractionButton.interactionMode == 1)
		{
			DialogueBox.GetNode<Label>("NameText").Text = "Look";
			DialogueBox.GetNode<RichTextLabel>("MainText").Text = LookDescription;
			DialogueBox.GetNode<Sprite2D>("FaceSprite").Texture = (Texture2D)GD.Load("res://Sprites/Faces/Eye.png");
			OpenDialogue();
		}
		if (InteractionButton.interactionMode == 2)
		{
			if (soundSource != null && DialogueSound != null)
			{
				soundSource.Stream = DialogueSound;
				soundSource.Play();
			}
			DialogueBox.GetNode<Label>("NameText").Text = npcName;
			DialogueBox.GetNode<RichTextLabel>("MainText").Text = Dialogue;
			DialogueBox.GetNode<Sprite2D>("FaceSprite").Texture = faceSprite;
			OpenDialogue();
		}
		if (InteractionButton.interactionMode == 3)
		{
			DialogueBox.GetNode<Label>("NameText").Text = "Touch";
			DialogueBox.GetNode<RichTextLabel>("MainText").Text = TouchDescription;
			DialogueBox.GetNode<Sprite2D>("FaceSprite").Texture = (Texture2D)GD.Load("res://Sprites/Faces/Touch.png");
			OpenDialogue();
		}
		isTalking = true;
	}

	public void CloseDialogue()
	{
		DialogueBox.Hide();
		isTalking = false;
	}
}
