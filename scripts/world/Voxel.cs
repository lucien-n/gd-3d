using Godot;

public partial class Voxel : Node
{
    public Vector3I position;
    int type;
    int material = VoxelMaterial.BASE;

    public Voxel(Vector3I position, int type, int material)
    {
        this.position = position;
        this.type = type;
        this.material = material;
    }
}