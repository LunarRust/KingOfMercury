extends CharacterBody3D

@export_category("Assignments")
@export var nav_agent : NavigationAgent3D
@export var anim : AnimationPlayer
@export var animTree : AnimationTree
@export var modelRoot : Node3D
@export var HealthHandler : Node3D
var TargetEntity

@export_category("Characteristics")
@export var hostile : bool = true
@export var speed: float = 3
@export var acceleration: float = 5
@export var FollowDistance : float


@export_category("Attack Data")
@export var AttackDistance : float = 1.5
@export var attackThreshold : float = 1.5
@export var attackPower : int = 1
@export var aggroRange : int = 10
@export var walkName = "Walk"
@export var attackName = "Attack"
@export var meleeAttack : bool = true


var attackTimer : float
var attacking : bool = false
var active : bool = false
var hurt : bool = false
var Tset : bool = false
var TargetIsItem : bool = false
var TargetIsCreature : bool = true



var playerHealth = load("res://Scripts/PlayerHealthHandler.cs")
var playerMover = load("res://Scripts/MoverTest.cs")
var Interactions = load("res://Scripts/Interactions.cs")
var playerHealthInstance = playerHealth.new()

func _ready():
	
	if (TargetEntity == null):
		print("Ouchie wawa! There's no defined player object for this enemy to chase! Trying to find one now.")
		TargetEntity = get_tree().get_first_node_in_group("player")
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Activate_Pomp_Target.connect(TargetEnimies)
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Activate_Player_Target.connect(TargetPlayer)
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Kill_pomp.connect(KillSelf)
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Item_Grab.connect(LocateItem)
		active = true
		nav_agent.target_desired_distance = FollowDistance
	else:
		active = true
	pass
		

func _physics_process(delta):
	if (active):
		if Input.is_physical_key_pressed(KEY_5):
			SetTarget()
		active_handling(delta)
		if Tset:
			SetTarget()
		if Input.is_physical_key_pressed(KEY_6):
			hostile = false
			TargetEntity = get_tree().get_first_node_in_group("player")
		get_tree().get_first_node_in_group("PompNpcStats").get_node("TargetLabel").set_text("Target is: [color=red]" + str(TargetEntity.name) + "[/color]")

func active_handling(delta):
	if (TargetEntity == null):
		hostile = false
		print("Ouchie wawa! There's no target for this enemy to chase! Trying to find one now.")
		TargetEntity = get_tree().get_first_node_in_group("player")
		TargetIsCreature = true
	#print("velocity less than 1: " + str(velocity.length() < 1.0) + " " + str(velocity.length()))
	if TargetIsCreature:
		if (position.distance_to(TargetEntity.position) < aggroRange && !attacking && !hurt):
			attacking = true
		if (position.distance_to(TargetEntity.position) > AttackDistance && attacking && !hurt):
			handle_Move(delta)
			if HealthHandler.CoreHealthHandler.HP > 5 && !anim.current_animation == "Attack" && !anim.current_animation == "AttackLow":
				animTrigger(walkName)
			elif HealthHandler.CoreHealthHandler.HP <= 5:
				animTrigger("WalkLow")
		elif !hurt && HealthHandler.CoreHealthHandler.HP > 5:
			velocity = velocity.lerp(Vector3.ZERO, delta)
			animTrigger("Idle")
		elif  !hurt && HealthHandler.CoreHealthHandler.HP <= 5:
			velocity = velocity.lerp(Vector3.ZERO, delta)
			animTrigger("IdleLow")
			
		if (position.distance_to(TargetEntity.position) < AttackDistance):
			attackTimer += 1 * delta

		if (attackTimer > attackThreshold && attacking && meleeAttack && hostile):
			Attack()
			attackTimer = 0
			
	if TargetIsItem:
		if (position.distance_to(TargetEntity.position) > AttackDistance && !hurt):
			handle_Move(delta)
			if HealthHandler.CoreHealthHandler.HP > 5 && !anim.current_animation == "Touch" && !anim.current_animation == "TouchLow":
				animTrigger(walkName)
			elif HealthHandler.CoreHealthHandler.HP <= 5:
				animTrigger("WalkLow")
		elif !hurt && HealthHandler.CoreHealthHandler.HP > 5:
			velocity = velocity.lerp(Vector3.ZERO, delta)
			animTrigger("Idle")
		elif  !hurt && HealthHandler.CoreHealthHandler.HP <= 5:
			velocity = velocity.lerp(Vector3.ZERO, delta)
			animTrigger("IdleLow")
			
		if (position.distance_to(TargetEntity.position) < AttackDistance):
			attackTimer += 1 * delta

		if (attackTimer > attackThreshold):
			GrabItem()
			attackTimer = 0
	

	if (animTree != null):
		animTree["parameters/Normal2D/blend_position"] = velocity.length() / speed
	
func SetTarget():
		TargetEntity = TargetLocator()
		hostile = true

func update_target_location(target_location):
	nav_agent.target_position = target_location
	
func handle_Move(delta):

	var direction = Vector3()
	nav_agent.target_position = TargetEntity.global_position
	direction = nav_agent.get_next_path_position() - global_position
	direction = direction.normalized()
	velocity = velocity.lerp(direction * speed, delta * acceleration)
	#velocity = velocity.move_toward(direction * speed, .25)
	move_and_slide()

	var lookTarget = Vector3(global_position.x + velocity.x, global_position.y, global_position.z + velocity.z)

	var targetPos: Vector2 = Vector2(lookTarget.x, lookTarget.z)
	var modelPos : Vector2 = Vector2(modelRoot.global_position.x, modelRoot.global_position.z)

	if(!nav_agent.is_target_reachable() && velocity.length() / speed < .3):
		targetPos = Vector2(TargetEntity.global_position.x, TargetEntity.global_position.z)

	var modelDir = -(modelPos - targetPos)
	modelRoot.global_rotation.y = lerp_angle(modelRoot.global_rotation.y, atan2(modelDir.x, modelDir.y), delta * 6)

func Attack():
	if (anim != null && TargetEntity.has_node("HealthHandler") && HealthHandler.CoreHealthHandler.HP > 5):
		animTrigger(attackName)
	elif (anim != null && TargetEntity.has_node("HealthHandler") && HealthHandler.CoreHealthHandler.HP < 5):
		animTrigger("AttackLow")
	if (position.distance_to(TargetEntity.position) < AttackDistance && TargetEntity.is_in_group("player")):
		playerHealthInstance.notsostatichealth(attackPower)
	else:
		if position.distance_to(TargetEntity.position) < AttackDistance && TargetEntity.has_node("NpcToNpcHealthHandler"):
			TargetEntity.get_node("NpcToNpcHealthHandler").Hurt(1)
		elif position.distance_to(TargetEntity.position) < AttackDistance && TargetEntity.has_node("HealthHandler"):
			TargetEntity.get_node("HealthHandler").Hurt(1)
	await get_tree().create_timer(1.0).timeout
	pass
	
func GrabItem():
	if (anim != null && HealthHandler.CoreHealthHandler.HP > 5):
		animTrigger("Touch")
	elif (anim != null && HealthHandler.CoreHealthHandler.HP < 5):
		animTrigger("TouchLow")
	await get_tree().create_timer(1.0).timeout
	pass

func TargetLocator():
	var NearestTarget
	for i in get_all_children(get_tree().get_root()):
		if "Innocent" in i:
			#print("possible target has innocent property")
			if !i.get_parent().is_in_group("PompNPC"):
				#print("possible target is not Fellow PompNPC")
				if NearestTarget == null:
					NearestTarget = i.get_parent()
				if i.get_parent().global_position.distance_to(self.global_position) < NearestTarget.get_parent().global_position.distance_to(self.global_position):
					NearestTarget = i.get_parent()
	Tset = false
	print_rich("new target: [color=red]" + (NearestTarget.name) + "[/color]")
	TargetIsCreature = true
	TargetIsItem = false
	return NearestTarget

func ItemLocator():
	var NearestTarget
	for i in get_all_children(get_tree().get_root()):
		if "ItemID" in i:
			if NearestTarget == null:
				NearestTarget = i.get_parent()
			if i.get_parent().global_position.distance_to(self.global_position) < NearestTarget.get_parent().global_position.distance_to(self.global_position):
				NearestTarget = i.get_parent()
	print_rich("new target: [color=red]" + (NearestTarget.name) + "[/color]")
	TargetIsCreature = false
	TargetIsItem = true
	return NearestTarget

func LocateItem():
	!hostile
	TargetEntity = ItemLocator()
	
func get_all_children(in_node, array := []):
	array.push_back(in_node)
	for child in in_node.get_children():
		array = get_all_children(child, array)
	return array

func animTrigger(triggername : String):
	animTree["parameters/conditions/" + triggername] = true;
	await get_tree().create_timer(0.1).timeout
	animTree["parameters/conditions/" + triggername] = false;
	
	
func TargetEnimies():
	Tset = true
	
func KillSelf():
	self.get_node("NpcToNpcHealthHandler").Hurt(99999)
	
func TargetPlayer():
	hostile = false
	TargetIsCreature = true
	TargetIsItem = false
	TargetEntity = get_tree().get_first_node_in_group("player")
