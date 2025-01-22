extends Node3D
var RCS : RayCastSystem = RayCastSystem.new()
var active : bool = false
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
		active = true
	else:
		active = false
	
