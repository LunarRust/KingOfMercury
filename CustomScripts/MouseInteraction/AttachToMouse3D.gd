extends Node3D
var RCS : RayCastSystem = RayCastSystem.new()
var active : bool = false
# Called when the node enters the scene tree for the first time.
func _ready():
	active = true
	pass # Replace with function body.
	
func _physics_process(delta):
	if active:
		self.global_position = RCS.get_mouse_world_position()
	

func active_handling(delta):
	pass
	
