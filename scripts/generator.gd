class_name Generator extends Node2D

@onready var box = preload("res://scenes/box.tscn")
@onready var chunks = $chunks

signal s_generate_chunk(position: Vector3)

const CHUNK_SIZE: int = 16
const PREGEN_CHUNK_QUANTITY: int = 2

var CHUNKS: Array[Vector3] = []

var noise = FastNoiseLite.new()
	
func generate_chunk(chunk_pos: Vector3):
	var chunk_world_pos = Vector3(chunk_pos.x * CHUNK_SIZE, 0, chunk_pos.z * CHUNK_SIZE)
	if (CHUNKS.has(chunk_pos)): return
	print("Generating chunk: ", chunk_world_pos, " || world: ", chunk_pos)
	
	var chunk = StaticBody3D.new()
	chunk.position = chunk_world_pos
	
	var abs_x = chunk.position.x
	var abs_z = chunk.position.z
	
	for x in range(CHUNK_SIZE):
		for z in range(CHUNK_SIZE):
			var new_box = box.instantiate()
			var y = noise.get_noise_2d(abs_x + x, abs_z + z)
			y = floor(y * 10)
			new_box.position = Vector3(x, y, z)
			chunk.add_child(new_box)
	print("Generated: ", chunk.position)
	print('='.repeat(24))
	add_child(chunk)
	CHUNKS.append(chunk_pos)

func _on_world_s_generate_chunk(position: Vector3):
	generate_chunk(position)
