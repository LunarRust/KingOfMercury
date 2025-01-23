extends Node
@export var Scene : PackedScene
@export var textbox : LineEdit
#@export var LocationRelative : Vector3
@export var TargetLoc : RayCast3D
@export var Property : String
@export var SourceProperty: Node
@export var Value : String
var id
var ScenePack
var start : bool = false
var Spawned = false
var textboxDest


func _ready():
	start = true
	pass
	
func _process(delta):
	if start == true:
		if self.get_parent().active:
			if !Spawned:
				Spawned = true
				Packload()

	
func create():
	id = self.get_parent().InstID
	var node : Node = ScenePack.instantiate()
	get_tree().current_scene.add_child(node)
	node.global_position = TargetLoc.get_collision_point()
	node.InstID = id
				
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
		get_tree().current_scene.add_child(node)
		node.global_position = TargetLoc.get_collision_point()
		print(node.get_tree_string_pretty())
		TargetLoc.get_collision_point()
