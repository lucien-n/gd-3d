extends Node3D

var vp: Viewport

func _init():
	RenderingServer.set_debug_generate_wireframes(true)
	
func _ready():
	vp = get_viewport()
	
func _input(event):
	if event is InputEventKey and Input.is_key_pressed(KEY_P):
		vp.debug_draw = (vp.debug_draw + 1) % 6
