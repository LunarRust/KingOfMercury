using Godot;

[ScriptPath("res://Scripts/EntitySpawner.cs")]
public partial class EntitySpawner : Node
{
	private PackedScene playerNode = GD.Load("res://prefabs/player_object.tscn") as PackedScene;

	private PackedScene eggNode = GD.Load("res://prefabs/eggofearthMOD.tscn") as PackedScene;

	private PackedScene keyNode = GD.Load("res://prefabs/key_pickup.tscn") as PackedScene;

	private PackedScene doorNode = GD.Load("res://prefabs/door.tscn") as PackedScene;

	private PackedScene syringeNode = GD.Load("res://prefabs/Syringe_pickup.tscn") as PackedScene;

	private PackedScene NpcNode = GD.Load("res://prefabs/npc_template.tscn") as PackedScene;

	private PackedScene EnemyNode = GD.Load("res://prefabs/Enemy_Template.tscn") as PackedScene;

	private PackedScene TransferNode = GD.Load("res://prefabs/transfer.tscn") as PackedScene;

	private PackedScene BloodGuard = GD.Load("res://prefabs/bloody_guard.tscn") as PackedScene;

	public override void _Ready()
	{
		objectCheck("player", playerNode);
		objectCheck("egg", eggNode);
		foreach (Node item in GetTree().GetNodesInGroup("ent"))
		{
			if (item.Name.ToString().StartsWith("ent_door"))
			{
				CreateObject(doorNode, item as Node3D);
			}
			if (item.Name.ToString().StartsWith("ent_key"))
			{
				CreateObject(keyNode, item as Node3D);
			}
			if (item.Name.ToString().StartsWith("ent_syringe"))
			{
				CreateObject(syringeNode, item as Node3D);
			}
			if (item.Name.ToString().StartsWith("ent_npc"))
			{
				CreateNPC(NpcNode, item as Node3D);
			}
			if (item.Name.ToString().StartsWith("ent_enemy"))
			{
				GD.Print("enemy type is " + item.Get("EnemyType").ToString());
				if ((string)item.Get("EnemyType") == "Custom" || (string)item.Get("EnemyType") == "")
				{
					CreateEnemy(EnemyNode, item as Node3D);
				}
				else
				{
					CreateEnemyPrefab(item as Node3D);
				}
			}
			if (item.Name.ToString().StartsWith("ent_transfer"))
			{
				CreateTransfer(TransferNode, item as Node3D);
			}
		}
	}

	public void CreateObject(PackedScene origin, Node3D entStandin)
	{
		GD.Print("Spawning entity at " + entStandin.Name);
		Node3D node3D = origin.Instantiate(PackedScene.GenEditState.Disabled) as Node3D;
		node3D.Position = entStandin.Position;
		node3D.Rotation = entStandin.Rotation;
		AddChild(node3D, forceReadableName: false, InternalMode.Disabled);
		entStandin.QueueFree();
	}

	public void objectCheck(string name, PackedScene prefab)
	{
		Node node = GetNode("%ent_" + name);
		if (node != null)
		{
			CreateObject(prefab, GetNode("%ent_" + name) as Node3D);
		}
		else
		{
			GD.Print("Was not able to find placeholder object for " + name);
		}
	}

	public void CreateNPC(PackedScene origin, Node3D entStandin)
	{
		GD.Print("Spawning NPC at " + entStandin.Name);
		Node3D node3D = origin.Instantiate(PackedScene.GenEditState.Disabled) as Node3D;
		node3D.Position = entStandin.Position;
		node3D.Rotation = entStandin.Rotation;
		node3D.GetChild(1).QueueFree();
		Node child = node3D.GetChild(2);
		child.Set("npcName", (string)entStandin.Get("NpcName"));
		child.Set("LookDescription", (string)entStandin.Get("LookDialogue"));
		child.Set("Dialogue", (string)entStandin.Get("TalkDialogue"));
		child.Set("TouchDescription", (string)entStandin.Get("TouchDialogue"));
		child.Set("lookTarget", node3D.Position + node3D.Basis.Z);
		node3D.GetChild(4).Set("HP", (int)entStandin.Get("Health"));
		AddChild(node3D, forceReadableName: false, InternalMode.Disabled);
		GD.Print(node3D.GetParent().Name);
		GD.Print(entStandin.GetParent().Name);
		entStandin.GetParent().CallDeferred("remove_child", entStandin);
		node3D.CallDeferred("add_child", entStandin);
		entStandin.Position = Vector3.Zero;
		entStandin.Rotation = Vector3.Zero;
	}

	public void CreateEnemy(PackedScene origin, Node3D entStandin)
	{
		GD.Print("Spawning Enemy at " + entStandin.Name);
		Node3D node3D = origin.Instantiate(PackedScene.GenEditState.Disabled) as Node3D;
		node3D.Position = entStandin.Position;
		node3D.Rotation = entStandin.Rotation;
		Node child = node3D.GetChild(3);
		child.Set("HP", (int)entStandin.Get("Health"));
		node3D.GetChild(1).QueueFree();
		node3D.Set("speed", entStandin.Get("Speed"));
		node3D.Set("attackThreshold", entStandin.Get("AttackThreshold"));
		node3D.Set("attackPower", entStandin.Get("Damage"));
		node3D.Set("AggroRange", entStandin.Get("AggroRange"));
		node3D.Set("walkName", entStandin.Get("walkAnimationName"));
		node3D.Set("attackName", entStandin.Get("attackAnimationName"));
		AddChild(node3D, forceReadableName: false, InternalMode.Disabled);
		entStandin.GetParent().CallDeferred("remove_child", entStandin);
		node3D.CallDeferred("add_child", entStandin);
		entStandin.Position = Vector3.Zero;
		entStandin.Rotation = Vector3.Zero;
		if ((Node)(GodotObject)entStandin.Get("model") != null)
		{
			Node node = (Node)(GodotObject)entStandin.Get("Model");
			if (node.GetChild(1) != null)
			{
				node3D.Set("anim", node.GetChild(1) as AnimationPlayer);
			}
		}
		if ((AudioStream)(GodotObject)entStandin.Get("enemySound") != null)
		{
			AudioStreamPlayer3D audioStreamPlayer3D = node3D.GetChild(4) as AudioStreamPlayer3D;
			audioStreamPlayer3D.Stream = (AudioStream)(GodotObject)entStandin.Get("enemySound");
			audioStreamPlayer3D.Play();
		}
	}

	public void CreateEnemyPrefab(Node3D entStandin)
	{
		PackedScene packedScene = null;
		if ((string)entStandin.Get("EnemyType") == "Blood Guard")
		{
			packedScene = BloodGuard;
		}
		GD.Print("Spawning Enemy at " + entStandin.Name);
		Node3D node3D = packedScene.Instantiate(PackedScene.GenEditState.Disabled) as Node3D;
		node3D.Position = entStandin.Position;
		node3D.Rotation = entStandin.Rotation;
		AddChild(node3D, forceReadableName: false, InternalMode.Disabled);
		entStandin.QueueFree();
	}

	public void CreateTransfer(PackedScene origin, Node3D entStandin)
	{
		GD.Print("Spawning Transfer at " + entStandin.Name);
		Node3D node3D = origin.Instantiate(PackedScene.GenEditState.Disabled) as Node3D;
		node3D.Position = entStandin.Position;
		node3D.Rotation = entStandin.Rotation;
		AddChild(node3D, forceReadableName: false, InternalMode.Disabled);
		node3D.Set("altScene", (string)entStandin.Get("destinationPath"));
		entStandin.QueueFree();
	}
}
