using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public partial class Chunk : StaticBody3D
{
    private const int CHUNK_SIZE = Global.CHUNK_SIZE;

    Dictionary data;
    public Vector3I chunk_position;

    private Task _task;

    public override void _Ready()
    {
        Position = chunk_position * CHUNK_SIZE;
        Name = chunk_position.ToString();
    }
}