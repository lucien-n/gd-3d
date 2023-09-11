using Godot;

public partial class Voxel : Node3D
{
    private MeshInstance3D _mesh;

    private Material _base;
    private Material _highlighted;

    public override void _Ready()
    {
        _mesh = GetNode<MeshInstance3D>("mesh");

        _base = GD.Load<Material>("res://materials/base.tres");
        _highlighted = GD.Load<Material>("res://materials/highlighted.tres");
    }

    public void OnAreaMouseEntered()
    {
        _mesh.MaterialOverride = _highlighted;
    }

    public void OnAreaMouseExited()
    {
        _mesh.MaterialOverride = _base;
    }
}