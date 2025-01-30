extends Node
@export_category("Assignments")
@export var PlayerCam : Camera3D
@export var MenuCam : Camera3D
@export var head : Node3D
@export var CanvasToShow : CanvasLayer
@export var CamCurve : Curve
@export var CollisionShape : CollisionShape3D
@export var MeshInstance : MeshInstance3D
@export var PlayerInvManager : Node3D
@export var MenuInvCtlGrid : CtrlInventoryGridEx
@export var UIToShow : Array[Node2D] = []


var PlayerInvCtlGrid

var used : bool = false
var t = 0.0
var MenuCamCurrentTransform
var PlayerCamCurrentTransform
var playerObject : Node3D

var hudmanager = load("res://prefabs/hudmanager.cs")

func _ready():
	CamCurve.bake_resolution = 100
	MenuCam.set_process(false)
	PlayerCam = get_viewport().get_camera_3d()
	playerObject = get_tree().get_first_node_in_group("player") as Node3D
	
	PlayerInvCtlGrid = PlayerInvManager.invCtrl
	MenuCamCurrentTransform = MenuCam.global_transform
	PlayerCamCurrentTransform = PlayerCam.global_transform
	
func Touch():
	MenuCamCurrentTransform = MenuCam.global_transform
	PlayerCamCurrentTransform = PlayerCam.global_transform
	
	
	print("CamGrab Touched!")
	if(!used):
		CameraGrab()
	
	
func _process(delta):
	if used:
		if Input.is_physical_key_pressed(KEY_SPACE):
			used = false
			#MenuCam.set_process(false)
			playerObject.set_process(true)
			PlayerInvManager.invCtrl = PlayerInvCtlGrid
			CanvasToShow.hide()
			if !UIToShow.is_empty():
				for i in UIToShow:
					i.hide()
			t = 0
			MeshInstance.global_position.y = MeshInstance.global_position.y - 500
			CollisionShape.global_position.y = CollisionShape.global_position.y - 500
			PlayerCam.global_transform = PlayerCam.get_parent().global_transform
			var tween
			tween = create_tween()
			tween.set_parallel()
			tween.tween_property(PlayerCam, "rotation", Vector3.ZERO, 0.5).set_trans(Tween.TRANS_QUAD)
			tween.tween_property(head, "rotation", Vector3.ZERO, 0.5).set_trans(Tween.TRANS_QUAD)
			#sPlayerCam.make_current()
			
		if CamCurve.sample(t) <= 1:
			t += delta
		PlayerCam.global_transform = PlayerCamCurrentTransform.interpolate_with(MenuCamCurrentTransform,CamCurve.sample(t))
	
	
func CameraGrab():
	MeshInstance.global_position.y = MeshInstance.global_position.y + 500
	CollisionShape.global_position.y = CollisionShape.global_position.y + 500
	CanvasToShow.show()
	PlayerInvManager.invCtrl = MenuInvCtlGrid
	if !UIToShow.is_empty():
		for i in UIToShow:
			i.show()
	print("Lever flipped!")
	used = true
