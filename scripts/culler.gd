extends Node

@onready var camera = get_parent().get_node("camera")
@onready var generator = get_parent().world.get_node('generator')
@onready var chunks = generator.get_node('chunks')

func _process(delta):
	distance()
	
func distance():
	var position = camera.position
	var x = floor(position.x / generator.CHUNK_SIZE) * generator.CHUNK_SIZE
	var z = floor(position.z / generator.CHUNK_SIZE) * generator.CHUNK_SIZE
	var y = 0
	
	for chunk in chunks.get_children():
		chunk.visible = chunk.position == Vector3(x, y, z)
	
func frustum():
	var space_state = camera.world.get_world_3d().direct_space_state
	for chunk in chunks.get_children():
		var query = PhysicsRayQueryParameters3D.create(chunk.position, camera.position)
		query.collide_with_bodies = true
		query.collide_with_areas = true
		var result: Dictionary = space_state.intersect_ray(query)
		if (!result.is_empty()): chunk.visible = false
