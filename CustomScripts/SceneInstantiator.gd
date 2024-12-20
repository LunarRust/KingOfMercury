extends TextureButton
@export var Scene : PackedScene
@export var LocationRelative : Vector3


func _on_pressed():
	create()
	
func create():
	var node : Node = Scene.instantiate()
	get_tree().current_scene.add_child(node)
	node.position = get_tree().get_first_node_in_group("player").global_position + LocationRelative
	print(node.get_tree_string_pretty())
