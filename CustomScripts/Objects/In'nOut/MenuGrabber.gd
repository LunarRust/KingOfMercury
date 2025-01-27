extends Node
@export var SoundSource : AudioStreamPlayer3D
@export var PlayerCam : Camera3D
@export var MenuCam : Camera3D
@export var delay : float
var used : bool = false
var t = 0.0
var MenuCamCurrentTransform
var PlayerCamCurrentTransform
var playerObject : Node3D
var hudmanager = load("res://prefabs/hudmanager.cs")

func _ready():
	MenuCam.set_process(false)
	PlayerCam = get_viewport().get_camera_3d()
	playerObject = get_tree().get_first_node_in_group("player") as Node3D
	MenuCamCurrentTransform = MenuCam.global_transform
	PlayerCamCurrentTransform = PlayerCam.global_transform
	
func Touch():
	MenuCamCurrentTransform = MenuCam.global_transform
	PlayerCamCurrentTransform = PlayerCam.global_transform
	
	print("Lever Touched!")
	if(!used):
		_OpenGate()
	pass
	
func _process(delta):
	if used:
		if Input.is_physical_key_pressed(KEY_SPACE):
			used = false
			#MenuCam.set_process(false)
			playerObject.set_process(true)
			hudmanager.ShowHUD()
			t = 0
			PlayerCam.global_transform = PlayerCamCurrentTransform
			#sPlayerCam.make_current()
		if t <= 1:
			t += delta
		PlayerCam.global_transform = PlayerCamCurrentTransform.interpolate_with(MenuCamCurrentTransform,t)
	
	
func _OpenGate():
	#playerObject.set_process(false)
	#MenuCam.set_process(true)
	#MenuCam.make_current()
	hudmanager.HideHUD()
	print("Lever flipped!")
	used = true
	SoundSource.play()
	await get_tree().create_timer(delay).timeout
	
	pass
