extends Node
@export var RegisterOrderGen : Node
@export var GeneratorID : int
var InnoutBus

# Called when the node enters the scene tree for the first time.
func _ready():
	InnoutBus = get_tree().get_first_node_in_group("InnOutSignalBus")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
	
func Reached():
	RegisterOrderGen.Generate(GeneratorID)
	if InnoutBus != null:
		InnoutBus.emit_signal("ReadyToOrder",GeneratorID)
	else:
		print("SignalBus does not exist!")
	print_rich("[color=red] Reached! [/color]")
