class_name Toucher
extends Node3D
var RCS : RayCastSystem = RayCastSystem.new()
var active : bool = false
var space_state
var cam
var mousepos
var CurrentIntersectedObject
var TouchedObject
var InteractionButton = preload("res://Scripts/InteractionButton.cs")
var ViewButton = preload("res://Scripts/ViewButton.cs")

# Called when the node enters the scene tree for the first time.
func _ready():
	active = true


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _physics_process(delta):
	space_state = get_world_3d().direct_space_state
	cam = get_viewport().get_camera_3d()
	mousepos = get_viewport().get_mouse_position()
	self.global_position = RCS.get_mouse_world_position(space_state,cam,mousepos)
	CurrentIntersectedObject = RCS.get_raycast_hit_object(space_state,cam,mousepos)
	
func Cast():
	var zDepth : float
	zDepth = 2
	var from : Vector3 = cam.project_ray_origin(mousepos)
	var to = cam.project_position(mousepos,zDepth)
	var physicsRayQueryParameters3D : PhysicsRayQueryParameters3D = PhysicsRayQueryParameters3D.create(from,to)
	physicsRayQueryParameters3D.hit_back_faces = false
	var dictionary : Dictionary = space_state.intersect_ray(physicsRayQueryParameters3D)
	print_rich("[color=red]" + str(dictionary) + "[/color]")
	if !dictionary.is_empty() && (dictionary["collider"] as CollisionObject3D).has_node("Behavior"):
		var node : Node = (dictionary["collider"] as CollisionObject3D).get_node("Behavior")
		if node.has_method("Touch"):
			node.Touch()
		print_rich("Touched object is: [color=red]" + str(node) + "[/color]")
	
	TouchedObject = CurrentIntersectedObject
	
	
func _input(event):
	if event is InputEventMouseButton && event.is_pressed():
		Cast()

func get_all_children(in_node, array := []):
	array.push_back(in_node)
	for child in in_node.get_children():
		array = get_all_children(child, array)
	return array
