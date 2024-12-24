extends Node


@export var Target : StaticBody3D
@export var ChildNumber : int
@export var Entrance : bool
@export var Exit : bool
@export var OnVolumeExit : bool
var opened : bool



func _on_area_entered(area):
	if !OnVolumeExit:
		print("area entered")
		print("is Entrance: " + str(Entrance) + " and is opened: " + str(opened) + " and is moving: " + str(Target.get_child(ChildNumber).moving))
		if !Target.get_child(ChildNumber).moving:
			match opened:
				true:
					if Exit:
						close()
				false:
					if Entrance:
						open()
			
		
func open():
	Target.get_child(ChildNumber).RemoteTriggerActivate()

func close():
	Target.get_child(ChildNumber).RemoteTriggerDeactivate()



func _on_behavior_open():
	print("Door opened")
	opened = true
	

func _on_behavior_closed():
	print("door closed")
	opened = false


func _on_area_exited(area):
	if !Target.get_child(ChildNumber).moving:
		if OnVolumeExit:
			print("area exited")
			print("is Entrance: " + str(Entrance) + " and is opened: " + str(opened) + " and is moving: " + str(Target.get_child(ChildNumber).moving))
			match opened:
				true:
					if Exit:
						close()
				false:
					if Entrance:
						open()
