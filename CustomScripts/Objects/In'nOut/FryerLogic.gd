extends Node
var model
@export var TargetLoc : Node3D
@export var Models : Array[Resource]


func Item(item : String):
	match item:
		"Fries":
			model = Models[0] as PackedScene
			return true
		"Burger":
			model = Models[1] as PackedScene
			return true
		_:
			return false


func Packload():
	var node : Node = model.instantiate()
	get_tree().current_scene.add_child(node)
	if TargetLoc is RayCast3D:
		node.global_position = TargetLoc.get_collision_point()
	else:
		node.global_position = TargetLoc.global_position
	print(node.get_tree_string_pretty())
