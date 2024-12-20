extends Node2D

@export var key : Key
@export var SoundEffect : AudioStreamPlayer
@export var OtherNodes : Array[Node2D]


func _process(delta):
	if Input.is_physical_key_pressed(key):
		($".".show())
		if !SoundEffect.playing:
			SoundEffect.play()
		if OtherNodes:		
			for i in OtherNodes:
				if i != null:
					i.show()
				#if i == null:
				#	print("cannot show: " + str(i) + " it is a null value!")
