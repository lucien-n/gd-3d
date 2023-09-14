using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public partial class Chunk : StaticBody3D
{
    private const int CHUNK_SIZE = Global.CHUNK_SIZE;

    public Dictionary<Vector3I, int> data = new();
    public Vector3I chunk_position;
    private Task _task;


    public override void _Ready()
    {
        Position = chunk_position * CHUNK_SIZE;
        Name = chunk_position.ToString();

        data = WorldGenerator.GenerateChunk(chunk_position);

        Regenerate();
    }

    public void Regenerate()
    {
        foreach (var c in GetChildren())
        {
            RemoveChild(c);
            c.QueueFree();
        }

        GenerateChunkCollider();
        GenerateChunkMesh();
    }

    private void GenerateChunkCollider()
    {
        if (data.Count == 0)
        {
            CreateBlockCollider(Vector3I.Zero);
            CollisionLayer = 0;
            CollisionMask = 0;
        }

        CollisionLayer = 0xFFFFFF;
        CollisionMask = 0xFFFFFF;

        foreach (Vector3I block_position in data.Keys)
        {
            int block_id = data[block_position];
            // do stuff with id (ex: no collision for water/lava)
        }
    }

    private void GenerateChunkMesh()
    {
        if (data.Count == 0) return;

        SurfaceTool surface_tool = new();
        surface_tool.Begin(Mesh.PrimitiveType.Triangles);

        foreach (Vector3I block_position in data.Keys)
        {
            int block_id = data[block_position];
            DrawBlockMesh(surface_tool, block_position, block_id);
        }

        surface_tool.GenerateNormals();
        surface_tool.GenerateTangents();
        surface_tool.Index();

        ArrayMesh array_mesh = surface_tool.Commit();
        MeshInstance3D mesh_instance = new()
        {
            Mesh = array_mesh,
        };
        AddChild(mesh_instance);
    }

    private void CreateBlockCollider(Vector3I block_sub_position)
    {
        CollisionShape3D collider = new()
        {
            Shape = new BoxShape3D(),

            Position = block_sub_position + Vector3.One / 2
        };

        AddChild(collider);
    }

    private void DrawBlockMesh(SurfaceTool surface_tool, Vector3I block_sub_position, int block_id)
    {
        Array<Vector3I> vertices = CalculateBlockVertices(block_sub_position);
        Array<Vector2I> uvs = CalculateBlockUvs(block_id);
        var top_uvs = uvs;
        var bottom_uvs = uvs;

        // ? Transparent blocks
        Vector3I other_block_position = block_sub_position + Vector3I.Left;
        int other_block_id = 0;
        if (other_block_position.X == -1)
            other_block_id = World.GetBlockGlobalPosition(block_sub_position);
        else if (data.ContainsKey(other_block_position)) other_block_id = data[other_block_position];
        if ((block_id != other_block_id) && IsBlockTransparent(other_block_id))
            DrawBlockFace(surface_tool, new Array<Vector3I> { vertices[2], vertices[0], vertices[3], vertices[1] }, uvs);

        other_block_position = block_sub_position + Vector3I.Right;
        other_block_id = 0;
        if (other_block_position.X == Global.CHUNK_SIZE)
            other_block_id = World.GetBlockGlobalPosition(block_sub_position);
        else if (data.ContainsKey(other_block_position)) other_block_id = data[other_block_position];
        if ((block_id != other_block_id) && IsBlockTransparent(other_block_id))
            DrawBlockFace(surface_tool, new Array<Vector3I> { vertices[7], vertices[5], vertices[6], vertices[4] }, uvs);

        other_block_position = block_sub_position + Vector3I.Forward;
        other_block_id = 0;
        if (other_block_position.Z == -1)
            other_block_id = World.GetBlockGlobalPosition(block_sub_position);
        else if (data.ContainsKey(other_block_position)) other_block_id = data[other_block_position];
        if ((block_id != other_block_id) && IsBlockTransparent(other_block_id))
            DrawBlockFace(surface_tool, new Array<Vector3I> { vertices[6], vertices[4], vertices[2], vertices[0] }, uvs);

        other_block_position = block_sub_position + Vector3I.Back;
        other_block_id = 0;
        if (other_block_position.X == Global.CHUNK_SIZE)
            other_block_id = World.GetBlockGlobalPosition(block_sub_position);
        else if (data.ContainsKey(other_block_position)) other_block_id = data[other_block_position];
        if ((block_id != other_block_id) && IsBlockTransparent(other_block_id))
            DrawBlockFace(surface_tool, new Array<Vector3I> { vertices[3], vertices[1], vertices[7], vertices[5] }, uvs);

        other_block_position = block_sub_position + Vector3I.Down;
        other_block_id = 0;
        if (other_block_position.Y == -1)
            other_block_id = World.GetBlockGlobalPosition(block_sub_position);
        else if (data.ContainsKey(other_block_position)) other_block_id = data[other_block_position];
        if ((block_id != other_block_id) && IsBlockTransparent(other_block_id))
            DrawBlockFace(surface_tool, new Array<Vector3I> { vertices[4], vertices[5], vertices[0], vertices[1] }, bottom_uvs);

        other_block_position = block_sub_position + Vector3I.Up;
        other_block_id = 0;
        if (other_block_position.Y == Global.CHUNK_SIZE)
            other_block_id = World.GetBlockGlobalPosition(block_sub_position);
        else if (data.ContainsKey(other_block_position)) other_block_id = data[other_block_position];
        if ((block_id != other_block_id) && IsBlockTransparent(other_block_id))
            DrawBlockFace(surface_tool, new Array<Vector3I> { vertices[2], vertices[3], vertices[6], vertices[7] }, top_uvs);
    }

    private void DrawBlockFace(SurfaceTool surface_tool, Array<Vector3I> vertices, Array<Vector2I> uvs)
    {
        // surface_tool.Add(uvs[1]); 
        surface_tool.SetUV(uvs[1]);
        surface_tool.AddVertex(vertices[1]);
        // surface_tool.Add(uvs[2]); 
        surface_tool.SetUV(uvs[2]);
        surface_tool.AddVertex(vertices[2]);
        // surface_tool.Add(uvs[3]); 
        surface_tool.SetUV(uvs[3]);
        surface_tool.AddVertex(vertices[3]);

        // surface_tool.Add(uvs[2]); 
        surface_tool.SetUV(uvs[2]);
        surface_tool.AddVertex(vertices[2]);
        // surface_tool.Add(uvs[1]); 
        surface_tool.SetUV(uvs[1]);
        surface_tool.AddVertex(vertices[1]);
        // surface_tool.Add(uvs[0]); 
        surface_tool.SetUV(uvs[0]);
        surface_tool.AddVertex(vertices[0]);
    }

    private Array<Vector2I> CalculateBlockUvs(int block_id)
    {
        return new Array<Vector2I> { new(1, 1), new(1, 2), new(2, 1), new(2, 2) };
    }

    private Array<Vector3I> CalculateBlockVertices(Vector3I pos)
    {
        return new Array<Vector3I>() {
            new(pos.X, pos.Y, pos.Z),
            new(pos.X, pos.Y, pos.Z + 1),
            new(pos.X, pos.Y + 1, pos.Z),
            new(pos.X, pos.Y + 1, pos.Z + 1),
            new(pos.X + 1, pos.Y, pos.Z),
            new(pos.X + 1, pos.Y, pos.Z + 1),
            new(pos.X + 1, pos.Y + 1, pos.Z),
            new(pos.X + 1, pos.Y + 1, pos.Z + 1),
        };
    }

    public static bool IsBlockTransparent(int block_id)
    {
        return block_id == 0;
    }
}