extends Node
@export_category("Assignments")
@export var inv : Inventory
static var inventoryInstance : Inventory
@export var invCtrl : CtrlInventoryGrid

@export var Labels : Node2D
static var instance
static var itemArray : Dictionary
var camCast : Camera3D
@export_category("Inventory Parameters")
const DEFAULT_SIZE: Vector2i = Vector2i(10, 10)
@export_category("Item Info")
@export var Protoset : ItemProtoset
@export var ItemCounts : Dictionary
var InvItemsList : Dictionary
var RelevantItems : Dictionary
#####EXTERNAL VARS#####
var InvSize : Vector2i
var InvFreeSpace
var RandList : Array
# Called when the node enters the scene tree for the first time.
func _ready():
	InvSize = inv._constraint_manager.get_grid_constraint().size
	InvFreeSpace = InvSize.x * InvSize.y
	InvItemsList = Protoset._prototypes
	ItemGen()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	Labels.get_child(1).set_text("Free Space:[color=red] " + str(InvFreeSpace) + "[/color]")
	
	
func ItemGen():
	print_rich("InvItemsList:[color=red] " + str(InvItemsList) + "[/color]")
	RandList = generate_sum_array(InvFreeSpace,1,4)
	var iterant = -1
	for i in ItemCounts:
		iterant += 1
		ItemCounts[i] = RandList[iterant]
	print_rich("InvCounts:[color=red] " + str(ItemCounts) + "[/color]")
	for i in InvItemsList:
		for ii in ItemCounts.size():
			if i == ItemCounts.keys()[ii]:
				print(str(i) + " matches " + str(ItemCounts.keys()[ii]))
				RelevantItems[i] = ItemCounts.values()[ii]
			
	Labels.get_child(2).set_text("Items:[color=red] " + str(RelevantItems) + "[/color]")
	print_rich("Items:[color=red] " + str(RelevantItems) + "[/color]")
	pass

func generate_sum_array(maxSum, factor, maxNumber):
	var sum = 0
	var generationRange = int(maxNumber/factor)          #this one for non 0 values
	var array = []
	var cont = true
	while cont:
		var n = randi() % generationRange + 1
		n *= factor
		sum += n
		if sum > maxSum:
			n -= sum - maxSum
			cont = false
		if n != 0:                           
			array.append(n)

	return array
