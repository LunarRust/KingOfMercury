extends Node

@onready var Playerfacing = get_tree().get_first_node_in_group("player").global_transform.basis.z
@export var impulseStrength : float
#var Interact : InteractionButton


func Touch():
	print("Ball touched!")
	Playerfacing = get_tree().get_first_node_in_group("player").global_transform.basis.z
	$"..".apply_central_impulse(Vector2(cos(Playerfacing), sin(Playerfacing)) * impulseStrength)
	
