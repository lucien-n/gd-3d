extends Control

@onready var fps_label = $debug/fps
@onready var player_pos_label = $debug/player_pos

@export var world: World

func _process(delta):
	fps_label.text = "FPS: " + str(Engine.get_frames_per_second())
	
	if !world: return
	
	player_pos_label.text = "POS: " + str(world.player.position)
