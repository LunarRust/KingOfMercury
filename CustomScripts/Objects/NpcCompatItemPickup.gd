extends Node


var inv : Inventory
var NpcInv : Inventory
var used : bool = false
@export var ItemID : String
# Called when the node enters the scene tree for the first time.
func _ready():
	inv = InventoryManager.inventoryInstance
	NpcInv = get_tree().get_first_node_in_group("PompNPC").get_node("InventoryManager")
	pass # Replace with function body.


func Touch(AmNpc = false):
	if AmNpc:
		if (!used):
			var newItem = NpcInv.create_and_add_item(ItemID)
			if (newItem != null):
				used = true
				self.get_parent().queue_free()
			else:
				print("Cannot Add Item, not enough Room")
	else:
		if (!used):
			var newItem = inv.create_and_add_item(ItemID)
			if (newItem != null):
				used = true
				self.get_parent().queue_free()
			else:
				print("Cannot Add Item, not enough Room")
