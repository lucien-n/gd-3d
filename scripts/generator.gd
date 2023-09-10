class_name Generator extends Node

@onready var box = preload("res://scenes/box.tscn")
@onready var chunks = $chunks

signal s_generate_chunk(x, z)

const CHUNK_SIZE: int = 16
const PREGEN_CHUNK_QUANTITY: int = 2

var CHUNKS: Array[Vector2] = []

var noise = FastNoiseLite.new()
	
func _ready():	
	for x in range(PREGEN_CHUNK_QUANTITY):
		for z in range(PREGEN_CHUNK_QUANTITY):
			generate_chunk(Vector2(x, z))

func generate_chunk(chunk_pos: Vector2):
	if (CHUNKS.has(chunk_pos)): return
	
	var chunk = StaticBody3D.new()
	var abs_z = chunk_pos.y * CHUNK_SIZE
	var abs_x = chunk_pos.x * CHUNK_SIZE
	chunk.position = Vector3(chunk_pos.x * CHUNK_SIZE, 0, chunk_pos.y * CHUNK_SIZE)
	for x in range(CHUNK_SIZE):
		for z in range(CHUNK_SIZE):
			var new_box = box.instantiate()
			var y = noise.get_noise_2d(abs_x + x, abs_z + z)
			y = floor(y * 10)
			new_box.position = Vector3(x, y, z)
			chunk.add_child(new_box)
	add_child(chunk)
	CHUNKS.append(chunk_pos)

func _on_world_s_generate_chunk(pos: Vector2):
	generate_chunk(pos)
