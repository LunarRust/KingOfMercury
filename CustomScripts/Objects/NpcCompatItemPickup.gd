extends Node


var inv : Inventory
var NpcInv : Inventory
var used : bool = false
@export var ItemID : String
# Called when the node enters the scene tree for the first time.
func _ready():
	inv = InventoryManager.inventoryInstance
	if inv == null:
		pass
	pass # Replace with function body.


func Touch(AmNpc = false):
	NpcInv = get_tree().get_first_node_in_group("PompNPC").get_node("InventoryGrid")
	if AmNpc && NpcInv != null:
		if (!used):
			if NpcInv.can_add_item(create_item(ItemID)):
				var newItem = NpcInv.create_and_add_item(ItemID)
				if (newItem != null):
					used = true
					self.get_parent().queue_free()
					return true
			else:
				print("Cannot Add Item, not enough Room")
				return false
			
	elif (!used):
		var newItem = inv.create_and_add_item(ItemID)
		if (newItem != null):
			used = true
			self.get_parent().queue_free()
			return true
		else:
			print("Cannot Add Item, not enough Room")
			return false

func create_item(prototype_id: String) -> InventoryItem:
	var item: InventoryItem = InventoryItem.new()
	item.protoset = NpcInv.item_protoset
	item.prototype_id = prototype_id
	return item
