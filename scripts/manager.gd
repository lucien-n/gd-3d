extends Node

var camera: Camera3D
var world: Node3D

const RAY_LENGTH = 100

func _ready():
	world = get_node("../world")
	camera = world.get_node('camera')

func _process(delta):
	if true: return
	
	var mouse_pos = get_viewport().get_mouse_position()
	if camera and mouse_pos:
		var from = camera.project_ray_origin(mouse_pos)
		var to = from + camera.project_ray_normal(mouse_pos) * RAY_LENGTH
		var space = world.get_world_3d().direct_space_state
		var ray_query = PhysicsRayQueryParameters3D.new()
		ray_query.from = from
		ray_query.to = to
		ray_query.collide_with_areas = true
		var raycast_result = space.intersect_ray(ray_query)
		
		if (raycast_result.size() > 0): print(raycast_result.get('position'))
