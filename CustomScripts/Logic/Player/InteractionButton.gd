class_name InteractionButtonKOM
extends TextureButton
@export var buttonMode : int
@export var cursorSprite : Texture2D
@export var particle : GPUParticles2D
@export var soundSource : AudioStreamPlayer
@export var clickSound : AudioStream
@export var blipSprite : Sprite2D
@export var KeyCode : Key
static var interactionMode : int


# Called when the node enters the scene tree for the first time.
func _ready():
	interactionMode = 1


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _pressed():
	soundSource.stream = clickSound
	soundSource.play()
	interactionMode = buttonMode
	particle.texture = cursorSprite
	print("Interaction button pressed! Mode switched to: " + str(interactionMode))
	if blipSprite != null:
		blipSprite.position = self.position
		
func _input(event):
	if Input.is_key_pressed(KeyCode) && interactionMode != buttonMode:
		_pressed()
