using Godot;

[ScriptPath("res://Scripts/InteractionButton.cs")]
public partial class InteractionButton : TextureButton
{
	[Export(PropertyHint.None, "")]
	public int buttonMode;

	[Export(PropertyHint.None, "")]
	public Texture2D cursorSprite;

	[Export(PropertyHint.None, "")]
	public GpuParticles2D particle;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer soundSource;

	[Export(PropertyHint.None, "")]
	public AudioStream clickSound;

	[Export(PropertyHint.None, "")]
	public Sprite2D blipSprite;

	[Export(PropertyHint.None, "")]
	public Godot.Key keyCode;

	public static int interactionMode;

	public override void _Ready()
	{
		interactionMode = 1;
	}

	public override void _Process(double delta)
	{
	}

	public override void _Pressed()
	{
		soundSource.Stream = clickSound;
		soundSource.Play();
		base._Pressed();
		interactionMode = buttonMode;
		particle.Texture = cursorSprite;
		GD.Print("Pressed interaction mode button! Interaction mode changed to " + interactionMode);
		if (blipSprite != null)
		{
			blipSprite.Position = base.Position;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsKeyPressed(keyCode) && interactionMode != buttonMode)
		{
			_Pressed();
		}
	}
}
