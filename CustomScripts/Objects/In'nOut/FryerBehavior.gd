extends Node
@export var FryBasketAnim : AnimationTree
@export var FryerSound : AudioStreamPlayer
var up = true
var model
@export var TargetLoc : Node3D
@export var Models : Array[Resource]


func Item(item : String):
	match item:
		"Fries":
			model = Models[0] as PackedScene
			Packload()
			return true
		"Burger":
			model = Models[1] as PackedScene
			Packload()
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

func Touch():
	FryerSound.play()
	if up:
		animTrigger("Down")
		up = false
	else:
		animTrigger("Up")
		up = true
	

func animTrigger(triggername : String):
	FryBasketAnim["parameters/conditions/" + triggername] = true;
	await get_tree().create_timer(0.1).timeout
	FryBasketAnim["parameters/conditions/" + triggername] = false;
