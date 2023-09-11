extends Control

@export var scene: Node3D

func _process(_delta):
	$debug/label.text = "Test Scene"
	$debug/camera.text = "Position: " + str(scene.get_node("camera").position)
