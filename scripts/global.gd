extends Node

const CHUNK_SIZE: int = 16
const RENDER_DISTANCE: int = 3

func world_to_chunk_position(position: Vector3) -> Vector3:
	return Vector3(floor(position.x / CHUNK_SIZE), 0, floor(position.z / CHUNK_SIZE))

func chunk_to_world_position(position: Vector3) -> Vector3:
	return Vector3(position.x * CHUNK_SIZE, 0, position.z * CHUNK_SIZE)
	
func round_to_multiple(x, multiple) -> float:
	return round(x / multiple) * multiple
