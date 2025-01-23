extends Node3D
var RCS : RayCastSystem = RayCastSystem.new()
var active : bool = false
@export var CharParent : Node3D
var space_state
var cam
var mousepos
# Called when the node enters the scene tree for the first time.
func _ready():
	if Input.is_mouse_button_pressed(2):
		active = true
		pass # Replace with function body.
	else:
		active = false
	
func _physics_process(delta):
	if active:
		space_state = get_world_3d().direct_space_state
		cam = get_viewport().get_camera_3d()
		mousepos = get_viewport().get_mouse_position()
		self.global_position = RCS.get_mouse_world_position(space_state,cam,mousepos)
	

func _process(delta):
	if Input.is_mouse_button_pressed(2):
		fade_in(CharParent,0.05)
		active = true
	else:
		fade_out(CharParent,1)
		active = false
	
	
func fade_out(node : Node3D,fade_duration : float):
	var tween = get_tree().create_tween()
	tween.tween_property(node, "modulate:a", 0, fade_duration)
	tween.play()
	await tween.finished
	tween.kill()

func fade_in(node : Node3D,fade_duration : float):
	var tween = get_tree().create_tween()
	tween.tween_property(node, "modulate:a", 1, fade_duration)
	tween.play()
	await tween.finished
	tween.kill()

