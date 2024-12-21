extends TextureButton
@export var Scene : PackedScene
#@export var LocationRelative : Vector3
@export var TargetLoc : RayCast3D


func _on_pressed():
	create()
	
func create():
	var node : Node = Scene.instantiate()
	get_tree().current_scene.add_child(node)
	node.global_position = TargetLoc.get_collision_point()
	print(node.get_tree_string_pretty())
	TargetLoc.get_collision_point()
