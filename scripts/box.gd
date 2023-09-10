extends Node3D

@onready var mesh = $mesh

@onready var base = preload("res://materials/base.tres")
@onready var selected = preload("res://materials/highlighted.tres")

func _on_area_mouse_entered():
	mesh.set_material_override(selected)


func _on_area_mouse_exited():
		mesh.set_material_override(base)
