extends Control

@onready var fps_label = $debug/fps
@onready var player_pos_label = $debug/player_pos
@onready var vp_drawing_mode_label = $debug/vp_drawing_mode
@onready var camera_label = $debug/camera

@export var world: Node3D

func _ready():
	var win_width =  ProjectSettings.get_setting("display/window/size/viewport_width")
	var win_height =  ProjectSettings.get_setting("display/window/size/viewport_height")
	scale = Vector2(win_width, win_height) / size

func _process(_delta):
	fps_label.text = "FPS: " + str(Engine.get_frames_per_second())
	vp_drawing_mode_label.text = "DRAW MODE: " + str(get_viewport().debug_draw)
	
	if !world: return
	
	player_pos_label.text = "POS: " + str(world.get_node("player").position)

	var camera_rotation = world.get_node("player").rotation
	var facing = "north"
	var ry = camera_rotation.y
	if ry < 0.5 and ry > -1:
		facing = "north"
	elif ry < -1 and ry > -2.5:
		facing = "east"
	elif ry < -2.5 or ry > 2.5:
		facing = "south"
	elif ry < 2.5 and ry > 1:
		facing = "ouest"
		
	camera_label.text = "CAMERA: " + str(camera_rotation) + " FACING: " + facing
