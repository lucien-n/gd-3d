class_name World extends Node3D

signal s_generate_chunk(x, z)

@onready var player = $player
@onready var generator = $generator

func _ready():
	pass

func _process(delta):
	var player_chunk_pos = position_to_chunk(Vector2(player.position.x, player.position.y))
	
	for chunk in generator.get_children():
		chunk.visible = chunk.position == Vector3(player_chunk_pos.x, 0, player_chunk_pos.y)
		s_generate_chunk.emit(player_chunk_pos)

func position_to_chunk(position: Vector2) -> Vector2:
	var x = floor(position.x / generator.CHUNK_SIZE)
	var z = floor(position.y / generator.CHUNK_SIZE)
	return Vector2(x, z)
	
func round_to_multiple(x, multiple) -> float:
	return floor(x / multiple) * multiple
