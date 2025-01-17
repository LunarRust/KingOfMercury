extends Node3D
@export var Scene : PackedScene
@export var textbox : LineEdit
#@export var LocationRelative : Vector3
@export var TargetLoc : Node3D
var ScenePack
var textboxDest


func _ready():
	Packload()
	
func create():
	var node : Node = ScenePack.instantiate()
	get_tree().current_scene.get_parent().add_child(node)
	node.global_position = TargetLoc.get_collision_point()
	print(node.get_tree_string_pretty())
	#TargetLoc.get_collision_point()
	
func Packload():
	if (textbox != null && textbox.text != "Enter prefab"):
			textboxDest = "res://prefabs/" + textbox.text + ".tscn"
			ResourceLoader.load_threaded_request(textboxDest)
			ScenePack = ResourceLoader.load_threaded_get(textboxDest) as PackedScene
			create()
	else:
		var node : Node = Scene.instantiate()
		get_tree().current_scene.get_parent().add_child(node)
		node.global_position = TargetLoc.get_collision_point()
		print(node.get_tree_string_pretty())
		TargetLoc.get_collision_point()
