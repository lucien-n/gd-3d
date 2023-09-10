extends Control

@onready var fps_label = $debug/fps
@onready var player_pos_label = $debug/player_pos

@export var world: Node3D

func _process(_delta):
	fps_label.text = "FPS: " + str(Engine.get_frames_per_second())
	
	if !world: return
	
	player_pos_label.text = "POS: " + str(world.player.position)
