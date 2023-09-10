extends Control

@onready var fps_label = $debug/fps

func _process(delta):
	fps_label.text = "FPS: " + str(Engine.get_frames_per_second())
