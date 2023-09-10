class_name World extends Node3D

signal s_generate_chunk(position: Vector3)

@onready var player = $player
@onready var generator = $generator

@export var render_distance = 2

func _process(delta):
	var player_chunk_pos = world_to_chunk_position(player.position)
	#for chunk in generator.get_children():
	#	chunk.visible = chunk.position == player_chunk_pos
	for x in range((render_distance * 2) + 1):
		for z in range((render_distance * 2) + 1):
			var pos = Vector3.ZERO
			pos.x = player_chunk_pos.x + x - render_distance
			pos.z = player_chunk_pos.z + z - render_distance
			s_generate_chunk.emit(pos)

func world_to_chunk_position(position: Vector3) -> Vector3:
	return Vector3(floor(position.x / generator.CHUNK_SIZE), 0, floor(position.z / generator.CHUNK_SIZE))

func chunk_to_world_position(position: Vector3) -> Vector3:
	return Vector3(position.x * generator.CHUNK_SIZE, 0, position.z * generator.CHUNK_SIZE)
	
func round_to_multiple(x, multiple) -> float:
	return round(x / multiple) * multiple
