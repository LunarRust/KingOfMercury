extends CharacterBody3D

@export_category("Assignments")
@export var nav_agent : NavigationAgent3D
@export var anim : AnimationPlayer
@export var animTree : AnimationTree
@export var modelRoot : Node3D
@export var HealthHandler : Node3D
@export var InvManager : Node3D
@export var FlashLight : SpotLight3D
@export var DebugLabelParent : Node3D
var TargetEntity
var DoLookAt : bool = false

@export_category("Characteristics")
@export var hostile : bool = true
@export var MaxSpeed: float = 3
var speed : float
@export var acceleration: float = 5
@export var MaxDistanceDef : float = 1.5
var MaxDistance : float


@export_category("Attack Data")
@export var attackThreshold : float = 1.5
@export var AttackDistance : float = 2
var AttackDistanceDefault : float
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
var velV2 : Vector2
var forwardVel : float
@onready var SoundSource : AudioStreamPlayer3D = self.get_node("AudioStreamPlayer3D")



var playerHealth = load("res://Scripts/PlayerHealthHandler.cs")
var playerMover = load("res://Scripts/MoverTest.cs")
var Interactions = load("res://Scripts/Interactions.cs")
var InteractableObject = load("res://Scripts/InteractableObject.cs")
var playerHealthInstance = playerHealth.new()

var LightSound = preload("res://Sounds/FlashLight.ogg")

func _ready():
	MaxDistance = MaxDistanceDef
	AttackDistanceDefault = AttackDistance
	speed = MaxSpeed
	if (TargetEntity == null):
		print("Ouchie wawa! There's no defined player object for this enemy to chase! Trying to find one now.")
		TargetEntity = get_tree().get_first_node_in_group("player")
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Activate_Pomp_Target.connect(TargetEnimies)
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Activate_Player_Target.connect(TargetPlayer)
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Kill_pomp.connect(KillSelf)
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Item_Grab.connect(LocateItem)
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Light_Toggle.connect(FlashLightToggle)
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Light_On.connect(FlashLightOn)
		get_tree().get_first_node_in_group("player").get_node("KOMSignalBus").Light_Off.connect(FlashLightOff)
		active = true
		nav_agent.target_desired_distance = MaxDistance
	else:
		active = true
	CheckGlobals()
		
func _physics_process(delta):
	if (active):
		if Input.is_physical_key_pressed(KEY_5):
			TargetEntity = TargetLocator()
		active_handling(delta)
		if Tset:
			TargetEntity = TargetLocator()
		if Input.is_physical_key_pressed(KEY_6):
			hostile = false
			TargetEntity = TargetLocator("player")
		if Input.is_mouse_button_pressed(2):
			hostile = false
			DoLookAt = false
			TargetEntity = TargetLocator("NpcMarker",1.2)
		get_tree().get_first_node_in_group("PompNpcStats").get_node("TargetLabel").set_text("Target is: [color=red]" + str(TargetEntity.name) + "[/color]")
		get_tree().get_first_node_in_group("PompNpcStats").get_node("TargetLabel2").set_text("Distance: [color=red]" + str(snapped(nav_agent.distance_to_target(),0.01)) + "[/color]")
		
		

func active_handling(delta):
	if (TargetEntity == null):
		hostile = false
		print("Ouchie wawa! There's no target for this enemy to chase! Trying to find one now.")
		TargetEntity = TargetLocator("player")
		DoLookAt = true
		TargetIsCreature = true
	#print("velocity less than 1: " + str(velocity.length() < 1.0) + " " + str(velocity.length()))
	if TargetIsCreature:
		if !TargetEntity.is_in_group("player") && !TargetEntity.is_in_group("PompNPC"):
			hostile = true
		if (position.distance_to(TargetEntity.position) < aggroRange && !attacking && !hurt):
			attacking = true
		if (position.distance_to(TargetEntity.position) > MaxDistance && attacking && !hurt):
			handle_Move(delta)
		elif !hurt:
			velocity = velocity.lerp(Vector3.ZERO, delta)
			animTree["parameters/Normal2D/4/blend_position"] = float(HealthHandler.CoreHealthHandler.HP)
			
		if (position.distance_to(TargetEntity.position) < AttackDistance):
			attackTimer += 1 * delta

		if (attackTimer > attackThreshold && attacking && hostile):
			Attack()
			attackTimer = 0
			
	if TargetIsItem:
		if (position.distance_to(TargetEntity.position) > MaxDistance && !hurt):
			handle_Move(delta)
		elif !hurt:
			velocity = velocity.lerp(Vector3.ZERO, delta)
			animTree["parameters/Normal2D/4/blend_position"] = float(HealthHandler.CoreHealthHandler.HP)
			
		if (position.distance_to(TargetEntity.position) < MaxDistance):
			attackTimer += 1 * delta

		if (attackTimer > attackThreshold && position.distance_to(TargetEntity.position) < MaxDistance):
			GrabItem()
			attackTimer = 0
	
	###
	### Calculations based on speed and velocity
	###
	@warning_ignore("shadowed_variable")
	var forwardVel = abs(velocity.dot(transform.basis.z)) + abs(velocity.dot(transform.basis.x))
	forwardVel = forwardVel
	if (position.distance_to(TargetEntity.position) < MaxDistance + 1.5):
		speed = (nav_agent.distance_to_target() - 1)
	else:
		speed = (nav_agent.distance_to_target()) + 0.286
	if speed >= MaxSpeed:
		speed = MaxSpeed
	if (nav_agent.distance_to_target() < MaxDistance + 3.5 && nav_agent.distance_to_target() > MaxDistance):
		if speed < 0.57:
			speed = 0.57
	
	velV2.y = forwardVel - 0.5
	if velV2.y < 0:
		velV2.y = 0
	
	if (animTree != null):
		animTree["parameters/Normal2D/blend_position"] = velV2
	
	if DoLookAt:
		velV2.x = find_rotation_to()
	else:
		velV2.x = 0
	DebugLabelParent.get_child(1).text = ("Blend X: " +  str(velV2.x))
	DebugLabelParent.get_child(0).text = ("Speed:  " +  str(speed))
	DebugLabelParent.get_child(2).text =("velocity: " +  str(velV2.y))

	if speed > 1.2:
		AttackDistance = 3
	else:
		AttackDistance = AttackDistanceDefault

func update_target_location(target_location):
	nav_agent.target_position = target_location
	
func handle_Move(delta):
	var direction = Vector3()
	nav_agent.target_position = TargetEntity.global_position
	direction = nav_agent.get_next_path_position() - global_position
	direction = direction.normalized()
	velocity = velocity.lerp(direction * speed, delta * acceleration)
	#velocity = velocity.move_toward(direction * speed, .25)
	var lookTarget = Vector3(global_position.x + velocity.x, global_position.y, global_position.z + velocity.z)
	var targetPos: Vector2 = Vector2(lookTarget.x, lookTarget.z)
	var modelPos : Vector2 = Vector2(modelRoot.global_position.x, modelRoot.global_position.z)
	if(!nav_agent.is_target_reachable() && velocity.length() / speed < .3):
		targetPos = Vector2(TargetEntity.global_position.x, TargetEntity.global_position.z)
	var modelDir = -(modelPos - targetPos)
	modelRoot.global_rotation.y = lerp_angle(modelRoot.global_rotation.y, atan2(modelDir.x, modelDir.y), delta * 4)
	move_and_slide()

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
	for i in get_all_children(TargetEntity):
		if (i.has_method("Touch")):
			i.Touch("AmNpc")
	pass

func FlashLightToggle():
	animTrigger("Flashlight");
	SoundSource.stream = LightSound
	SoundSource.play()
	if FlashLight.visible:
		FlashLight.visible = false
	else:
		FlashLight.visible = true
	print_rich("Flashlight toggled: [color=red]" + str(FlashLight.visible) + "[/color]")
	
func FlashLightOff():
	animTrigger("Flashlight");
	SoundSource.stream = LightSound
	SoundSource.play()
	FlashLight.visible = false
	print_rich("Flashlight toggled: [color=red]" + "OFF" + "[/color]")
	
func FlashLightOn():
	animTrigger("Flashlight");
	SoundSource.stream = LightSound
	SoundSource.play()
	FlashLight.visible = true
	print_rich("Flashlight toggled: [color=red]" + "ON" + "[/color]")
	
func CheckGlobals():
	if get_tree().get_first_node_in_group("NpcSceneRules") != null:
		if !get_tree().get_first_node_in_group("NpcSceneRules").FlashLightsEnabled:
			if FlashLight.visible:
				FlashLightOff()

func TargetLocator(SpefTarget = "default",MaxDist = MaxDistanceDef):
	var NearestTarget
	if MaxDist != MaxDistanceDef:
		MaxDistance = MaxDist
	else:
		MaxDistance = MaxDistanceDef
	if SpefTarget != "default":
		NearestTarget = find_closest_or_furthest(self,SpefTarget)
		DoLookAt = true
	else:
		NearestTarget = find_closest_or_furthest(self)
		DoLookAt = true
	Tset = false
	if NearestTarget != null:
		print_rich("new target: [color=red]" + (NearestTarget.name) + "[/color]")
		TargetIsCreature = true
		TargetIsItem = false
		return NearestTarget
	else:
		hostile = false
		NearestTarget = self
		print_rich("new target: [color=red] NULL" + "[/color]")
		DoLookAt = false
		animTrigger("Shrug")
		return NearestTarget

func ItemLocator():
	var NearestTarget
	DoLookAt = true
	NearestTarget = find_closest_or_furthest(self,"default",true)
	if NearestTarget != null:
		print_rich("new target: [color=red]" + (NearestTarget.name) + "[/color]")
		TargetIsCreature = false
		TargetIsItem = true
		for i in get_all_children(NearestTarget):
			if self.get_node("InventoryGrid").can_add_item(create_item(i.get_node("Behavior").ItemID)):
				return NearestTarget
			else:
				print("Inventory is full!")
				animTrigger("Shrug")
				DoLookAt = false
				NearestTarget = self
				return NearestTarget
	else:
		print("Item is Null!")
		animTrigger("Shrug")
		hostile = false
		DoLookAt = false
		NearestTarget = self
		return NearestTarget

func LocateItem():
	hostile = false
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
	
func create_item(prototype_id: String) -> InventoryItem:
	var item: InventoryItem = InventoryItem.new()
	item.protoset = InvManager.inv.item_protoset
	item.prototype_id = prototype_id
	return item

func TargetEnimies():
	Tset = true
	
func KillSelf():
	self.get_node("NpcToNpcHealthHandler").Hurt(99999)
	
func TargetPlayer():
	hostile = false
	TargetIsCreature = true
	TargetIsItem = false
	DoLookAt = true
	TargetEntity = get_tree().get_first_node_in_group("player")
	
func find_closest_or_furthest(node: Object,group_name = "default",item = false, get_closest:= true) -> Object:
	@warning_ignore("unassigned_variable")
	var PossibleTargets : Array
	if group_name == "default" && item == false:
		for i in get_all_children(get_tree().get_root()):
			if i.is_class("Node3D"):
				if i.has_method("Hurt"):
					if !i.get_parent().is_in_group("player") && !i.is_in_group("player"):
						if !i.get_parent().is_in_group("PompNPC"):
							PossibleTargets.append(i.get_parent())
		if !PossibleTargets.is_empty():
			var target_group = PossibleTargets
			var distance_away = node.global_transform.origin.distance_to(target_group[0].global_transform.origin)
			var return_node = target_group[0]
			for index in target_group.size():
				var distance = node.global_transform.origin.distance_to(target_group[index].global_transform.origin)
				if get_closest == true && distance < distance_away:
					distance_away = distance
					return_node = target_group[index]
				elif get_closest == false && distance > distance_away:
					distance_away = distance
					return_node = target_group[index]
			return return_node
		else:
			animTrigger("Shrug")
			return null
	if item == false:
		var target_group = get_tree().get_nodes_in_group(group_name)
		var distance_away = node.global_transform.origin.distance_to(target_group[0].global_transform.origin)
		var return_node = target_group[0]
		for index in target_group.size():
			var distance = node.global_transform.origin.distance_to(target_group[index].global_transform.origin)
			if get_closest == true && distance < distance_away:
				distance_away = distance
				return_node = target_group[index]
			elif get_closest == false && distance > distance_away:
				distance_away = distance
				return_node = target_group[index]
		return return_node
	else:
		if group_name == "default" && item == true:
			for i in get_all_children(get_tree().get_root()):
				if "ItemID" in i:
					PossibleTargets.append(i.get_parent())
			if !PossibleTargets.is_empty():
				var target_group = PossibleTargets
				var distance_away = node.global_transform.origin.distance_to(target_group[0].global_transform.origin)
				var return_node = target_group[0]
				for index in target_group.size():
					var distance = node.global_transform.origin.distance_to(target_group[index].global_transform.origin)
					if get_closest == true && distance < distance_away:
						distance_away = distance
						return_node = target_group[index]
					elif get_closest == false && distance > distance_away:
						distance_away = distance
						return_node = target_group[index]
				return return_node
			else:
				animTrigger("Shrug")
				return null
		else:
			animTrigger("Shrug")
			return null

func find_rotation_to(degree = false):
	var pos1 = self.global_transform.origin
	var pos2 = TargetEntity.global_transform.origin
	pos2.y = 2
	# Calculate the direction vector from node1 to node2 and normalize it
	var direction_to_node2 = (pos2 - pos1).normalized()
	var forward_vector = modelRoot.global_transform.basis.z.normalized()
	# Calculate the angle between the forward vector and the direction vector
	var angle = forward_vector.angle_to(direction_to_node2)
	# Use cross product to determine the angle's sign (relative to the Y-axis)
	var cross_product = forward_vector.cross(direction_to_node2)
	# Adjust angle based on the cross product's Y-component
	if cross_product.y < 0:
			angle = -angle
	# Convert the angle to degrees (if needed)
	var angle_degrees = rad_to_deg(angle)
	if(degree):
		return angle_degrees
	else:
		return angle
