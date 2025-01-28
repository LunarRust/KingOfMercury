extends Node
@export var PlayerCam : Camera3D
@export var MenuCam : Camera3D
@export var UIToShow : CanvasLayer
@export var CamCurve : Curve
@export var delay : float
@export var CollisionShape : CollisionShape3D
@export var MeshInstance : MeshInstance3D
var used : bool = false
var t = 0.0
var MenuCamCurrentTransform
var PlayerCamCurrentTransform
var playerObject : Node3D
@export var head : Node3D
@export var camera : Camera3D
var hudmanager = load("res://prefabs/hudmanager.cs")

func _ready():
	CamCurve.bake_resolution = 100
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
			UIToShow.hide()
			t = 0
			MeshInstance.global_position.y = MeshInstance.global_position.y - 5
			CollisionShape.global_position.y = CollisionShape.global_position.y - 5
			PlayerCam.global_transform = PlayerCam.get_parent().global_transform
			var tween
			tween = create_tween()
			tween.set_parallel()
			tween.tween_property(camera, "rotation", Vector3.ZERO, 0.5).set_trans(Tween.TRANS_QUAD)
			tween.tween_property(head, "rotation", Vector3.ZERO, 0.5).set_trans(Tween.TRANS_QUAD)
			#sPlayerCam.make_current()
			
		if CamCurve.sample(t) <= 1:
			t += delta
		PlayerCam.global_transform = PlayerCamCurrentTransform.interpolate_with(MenuCamCurrentTransform,CamCurve.sample(t))
	
	
func _OpenGate():
	MeshInstance.global_position.y = MeshInstance.global_position.y + 5
	CollisionShape.global_position.y = CollisionShape.global_position.y + 5
	#playerObject.set_process(false)
	#MenuCam.set_process(true)
	#MenuCam.make_current()
	hudmanager.HideHUD()
	UIToShow.show()
	print("Lever flipped!")
	used = true
	await get_tree().create_timer(delay).timeout
	
	pass
