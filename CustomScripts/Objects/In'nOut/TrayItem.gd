extends Node
@export var Scene : PackedScene
@export var TargetLoc : Node3D
@export var distance : float
var ScenePack
var currentID
var currentMark
var currentNPC
var SignalBusKOM
@export var NavNodeTarget : Node


func _ready():
	SignalBusKOM = get_tree().get_first_node_in_group("player").get_node("KOMSignalBus")

func Item(item : String):
	match item:
		"Fries":
			Scene = load("res://KOMPrefabs/Items/Fries_pickup.tscn") as PackedScene
			Packload()
			return true
		_:
			return false
	
			

# Called when the node enters the scene tree for the first time.
func Packload():
		var node : Node = Scene.instantiate()
		get_tree().current_scene.add_child(node)
		node.global_position = TargetLoc.global_position
		print(node.get_tree_string_pretty())
		
		NavNodeTarget = node
		await get_tree().create_timer(0.1).timeout

		currentNPC = find_closest_or_furthest(self.get_parent(),"PompNPC")
		currentID = currentNPC.InstID
		currentMark = get_tree().get_first_node_in_group("NavMark" + str(currentID))
		currentNPC.MaxSpeed = 2
		print_rich("Spawner Current ID: [color=red]" + str(currentID) + "[/color]")
		for i in get_all_children(get_tree().get_root()):
			if i.is_in_group("PompNPC"):
				if i.InstID == currentID:
					SignalBusKOM.emit_signal("ItemSpef",currentID,NavNodeTarget,0)
		
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
				print(str(PossibleTargets))
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

