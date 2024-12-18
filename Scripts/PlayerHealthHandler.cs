using System;
using System.Threading.Tasks;
using Godot;

[ScriptPath("res://Scripts/PlayerHealthHandler.cs")]
public partial class PlayerHealthHandler : Node
{
	public static int health = 9;

	public static int mana = 16;

	public static bool dead = false;

	[Export(PropertyHint.None, "")]
	public Timer manaTimer;

	[Export(PropertyHint.None, "")]
	public AnimationTree playerAnim;

	[Export(PropertyHint.None, "")]
	public MeshInstance3D playerBody;

	[Export(PropertyHint.None, "")]
	public BaseMaterial3D skin1;

	[Export(PropertyHint.None, "")]
	public BaseMaterial3D skin2;

	[Export(PropertyHint.None, "")]
	public BaseMaterial3D skin3;

	public static PlayerHealthHandler instance;

	[Export(PropertyHint.None, "")]
	public Sprite2D healthBar;

	[Export(PropertyHint.None, "")]
	public Sprite2D manaBar;

	[Export(PropertyHint.None, "")]
	public Sprite2D lowHealthOverlay;

	[Export(PropertyHint.None, "")]
	public Sprite2D faceSprite;

	[Export(PropertyHint.None, "")]
	public Label label;

	[Export(PropertyHint.None, "")]
	public Label manaLabel;

	[Export(PropertyHint.None, "")]
	public AnimationPlayer deathAnim;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer soundSource;

	private GDScript Fader = (GDScript)GD.Load("res://addons/UniversalFade/Fade.gd");

	public static bool usedHeal;

	public override void _Ready()
	{
		usedHeal = false;
		InventoryButton.open = false;
		instance = this;
		health = 9;
		changeHealth(0);
	}

	public override void _Process(double delta)
	{
		if (InventoryButton.open)
		{
			label.Text = health.ToString();
			manaLabel.Text = mana.ToString();
		}
		manaBar.Frame = 9 - mana / 2;
		mana = Math.Clamp(mana, 1, 16);
	}

	public void AddMana()
	{
		if (!MoverTest.running && !Input.IsActionPressed("Run") && mana < 16)
		{
			mana++;
		}
		manaTimer.Start(0.3499999940395355);
	}

	public void changeHealth(int amount)
	{
		if (dead)
		{
			return;
		}
		int num = health;
		if (amount < 0)
		{
			if (playerAnim != null)
			{
				AnimTrigger("Hurt");
			}
			CameraShake.Shake(0.1f);
			soundSource.Stream = GD.Load("res://Sounds/Hurt1.ogg") as AudioStream;
			soundSource.Play();
		}
		else if (amount > 0)
		{
			soundSource.Stream = GD.Load("res://Sounds/Pickup.ogg") as AudioStream;
			soundSource.Play();
			usedHeal = true;
		}
		health += amount;
		health = Mathf.Clamp(health, 0, 9);
		healthBar.Frame = 9 - health;
		healthCheck();
		FaceCheck();
	}

	public void FaceCheck()
	{
		if (playerAnim != null)
		{
			playerAnim.Set("parameters/Normal2D/4/blend_position", health);
		}
		if (health > 6)
		{
			if (playerBody != null)
			{
				playerBody.MaterialOverride = skin1;
			}
			faceSprite.Frame = 0;
		}
		else if (health > 5)
		{
			if (playerBody != null)
			{
				playerBody.MaterialOverride = skin2;
			}
			faceSprite.Frame = 1;
		}
		else if (health > 3)
		{
			faceSprite.Frame = 2;
		}
		else
		{
			if (playerBody != null)
			{
				playerBody.MaterialOverride = skin3;
			}
			faceSprite.Frame = 3;
		}
	}

	public static void staticHealth()
	{
		instance.changeHealth(-1);
	}

	public void notsostatichealth(int amount)
	{
		instance.changeHealth(amount * -1);
	}

	public void setHealth()
	{
		health = 9;
	}

	public void healthCheck()
	{
		if (health < 1 || (mana < 1 && !dead))
		{
			Death();
			dead = true;
		}
		if (health == 1)
		{
			Tween tween = CreateTween();
			tween.TweenProperty(lowHealthOverlay, "modulate", new Color(1f, 1f, 1f, 0.75f), 2.0);
		}
		else
		{
			Tween tween2 = CreateTween();
			tween2.TweenProperty(lowHealthOverlay, "modulate", new Color(1f, 1f, 1f, 0f), 2.0);
		}
	}

	public async void Death()
	{
		if (playerAnim != null)
		{
			AnimTrigger("Death");
		}
		deathAnim.Play("Death");
		dead = true;
		hudmanager.HideHUD();
		GetViewport().GetCamera3D().Position -= Vector3.Up;
		CameraCast.instance.Rotation = new Vector3(0f, 0f, 1.5708f);
		await Task.Delay(TimeSpan.FromSeconds(4.199999809265137));
		Fader.Call("crossfade_prepare", 3, "WeirdWipe", false, false);
		GetTree().ChangeSceneToPacked((PackedScene)GD.Load("res://Scenes/GameOver.tscn"));
		Fader.Call("crossfade_execute");
		dead = false;
		GD.Print("You died, buster!");
	}

	public async void AnimTrigger(string triggerName)
	{
		if (playerAnim != null)
		{
			playerAnim.Set("parameters/conditions/" + triggerName, true);
			await Task.Delay(TimeSpan.FromSeconds(0.1));
			if (GodotObject.IsInstanceValid(playerAnim))
			{
				playerAnim.Set("parameters/conditions/" + triggerName, false);
			}
		}
	}
}
