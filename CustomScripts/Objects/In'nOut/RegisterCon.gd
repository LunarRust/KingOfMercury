extends Node
var currentID
var currentMark
var currentNPC
var SignalBusKOM
var NpcInventory
@export var NavNodeTarget : Node
@export var SoundSource : AudioStreamPlayer
@export var SoundPositive : AudioStream
@export var SoundNegative : AudioStream
@export var PosRefrence : Node3D
@export var FoodList : Dictionary
@export var NeededAmounts : Array[int]
var NeededTotal : int
# Called when the node enters the scene tree for the first time.
func _ready():
	SignalBusKOM = get_tree().get_first_node_in_group("player").get_node("KOMSignalBus")
	pass # Replace with function body.


func NpcInvCheck():
	var NeededTotal = 0
	currentNPC = find_closest_or_furthest(PosRefrence,"PompNPC")
	currentID = currentNPC.InstID
	currentMark = get_tree().get_first_node_in_group("NavMark" + str(currentID))
	print_rich("Register Current ID: [color=red]" + str(currentID) + "[/color]")
	for i in get_all_children(get_tree().get_root()):
		if i.is_in_group("PompNPC"):
			if i.InstID == currentID:
				for ii in i.get_children():
					if ii.name == "InventoryGrid":
						NpcInventory = ii
	for i in FoodList.keys():
		FoodList[i] = 0
	
	for i in NpcInventory.get_children():
		var iterant = -1
		if "prototype_id" in i:
			for ii in FoodList.size():
				iterant += 1
				print("ItemID is: " + str(i.prototype_id) + " " + "FoodList Key is: " + str(FoodList.keys()[iterant]))
				if i.prototype_id == FoodList.keys()[iterant]:
					print("Match!")
					FoodList[FoodList.keys()[iterant]] += 1
					print(str(FoodList.values()))
	var TotalItems = 0
	for i in FoodList.size():
		if FoodList.values()[i] >= NeededAmounts[i]:
			TotalItems += 1
		else:
			print(str("not enough " + str(FoodList.values()[i])))
	for i in NeededAmounts:
		NeededTotal += NeededAmounts[i]
		
	print(str(TotalItems) + " " + str(NeededTotal))
	if TotalItems >= NeededTotal:
		return true
	else:
		return false

func Task():
	currentNPC = find_closest_or_furthest(PosRefrence,"PompNPC")
	currentID = currentNPC.InstID
	currentMark = get_tree().get_first_node_in_group("NavMark" + str(currentID))
	print_rich("Register Current ID: [color=red]" + str(currentID) + "[/color]")
	for i in get_all_children(get_tree().get_root()):
		if i.is_in_group("PompNPC"):
			if i.InstID == currentID:
				SignalBusKOM.emit_signal("NavToPoint",currentID,false,NavNodeTarget,1,0,"default")
	
	currentMark.global_position = NavNodeTarget.global_position
	currentMark = null
	currentID = null

func find_closest_or_furthest(node: Object,group_name,get_closest:= true) -> Object:
	@warning_ignore("unassigned_variable")
	var PossibleTargets : Array
	for i in get_all_children(get_tree().get_root()):
		if i.is_class("Node3D"):
			if i.is_in_group(group_name):
				PossibleTargets.append(i)
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
		return null
			
func get_all_children(in_node, array := []):
	array.push_back(in_node)
	for child in in_node.get_children():
		array = get_all_children(child, array)
	return array


func _on_pressed():
	
	if NpcInvCheck() == true:
		SoundSource.stream = SoundPositive
		SoundSource.play()
		Task()
	else:
		SoundSource.stream = SoundNegative
		SoundSource.play()
