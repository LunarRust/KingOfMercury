extends Node
var currentID
var currentMark
var SignalBusKOM
# Called when the node enters the scene tree for the first time.
func _ready():
	SignalBusKOM = get_tree().get_first_node_in_group("player").get_node("KOMSignalBus")
	pass # Replace with function body.


func Touch():
	currentMark = find_closest_or_furthest(self.get_parent(),"NavNode").get_node("NavMark")
	currentID = currentMark.get_parent().InstID
	for i in get_all_children(get_tree().get_root()):
		if i.is_in_group("PompNPC"):
			if i.InstID == currentID:
				SignalBusKOM.emit_signal("NavToPoint",currentID)
		if i.is_in_group("Table1"):
			currentMark.global_position = i.global_position

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
