extends Node2D

@export var key : Key
@export var SoundEffect : AudioStreamPlayer
# Called when the node enters the scene tree for the first time.
func _ready():
	
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if Input.is_physical_key_pressed(key):
		($".".show())
		if !SoundEffect.playing:
			SoundEffect.play()
	pass
