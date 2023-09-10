extends Node3D

@onready var box = preload("res://scenes/box.tscn")
@onready var box_manager = $box_manager

const SIZE: int = 8

func _ready():
	for x in range(SIZE):
		x -= SIZE / 2
		for y in range(SIZE):
			y -= SIZE / 2
			for z in range(SIZE):
				var new_box = box.instantiate()
				var position = Vector3(x, -z, y)
				new_box.position = position
				box_manager.add_child(new_box)

func _process(delta):
	pass
