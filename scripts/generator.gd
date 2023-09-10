class_name Generator extends Node2D

@onready var box = preload("res://scenes/box.tscn")
@export var player: Player

const CHUNK_SIZE: int = Global.CHUNK_SIZE
const RENDER_DISTANCE: int = Global.RENDER_DISTANCE 

var chunks: Dictionary = {}
var unready_chunks: Dictionary = {}

var thread = Thread.new()
var noise = FastNoiseLite.new()

func _process(_delta):
	update_chunks()
	
func update_chunks():
	var render_chunks: Array[Vector3] = []
	var player_chunk_pos = Global.world_to_chunk_position(player.position)
	
	for x in range((RENDER_DISTANCE * 2) + 1):
		for z in range((RENDER_DISTANCE * 2) + 1):
			var chunk_pos = Vector3.ZERO
			chunk_pos.x = player_chunk_pos.x + x - RENDER_DISTANCE
			chunk_pos.z = player_chunk_pos.z + z - RENDER_DISTANCE
			render_chunks.append(chunk_pos)
			if !chunks.has(chunk_pos) and !unready_chunks.has(chunk_pos) and !thread.is_started() :
				unready_chunks[chunk_pos] = true
				var callable = Callable(self, "generate_chunk")
				callable = callable.bind(chunk_pos)
				thread.start(callable)
	
	for chunk_pos in chunks.keys():
		chunks[chunk_pos].visible = render_chunks.has(chunk_pos)
	
func generate_chunk(chunk_pos: Vector3):
	var chunk_world_pos = Vector3(chunk_pos.x * CHUNK_SIZE, 0, chunk_pos.z * CHUNK_SIZE)
	
	var chunk = StaticBody3D.new()
	chunk.position = chunk_world_pos
	
	var abs_x = chunk_world_pos.x
	var abs_z = chunk_world_pos.z 
	
	for x in range(16):
		for z in range(16):
			var new_voxel = box.instantiate()
			var y = noise.get_noise_2d(abs_x + x, abs_z + z)
			y = floor(y * 10)
			new_voxel.position = Vector3(x, y, z)
			chunk.add_child(new_voxel)
	call_deferred("finish_chunk", chunk, chunk_pos)
	
func finish_chunk(chunk, chunk_pos: Vector3):
	chunks[chunk_pos] = chunk
	add_child(chunk)
	unready_chunks.erase(chunk_pos)
	
	if (thread.is_started()): thread.wait_to_finish()
