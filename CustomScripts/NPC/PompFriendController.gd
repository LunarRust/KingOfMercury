extends Node
signal AttackEnimeies
signal FollowPlayer


func _on_attack_button_pressed():
	KOMSignalBus.emit_signal("Activate_Pomp_Target")
