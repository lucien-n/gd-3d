using System.Collections;
using System.Linq;
using Godot;
using Godot.Collections;

public class Chunk
{
    public Vector3I chunk_position = Vector3I.Zero;
    public Vector3I world_position = Vector3I.Zero;
    public Hashtable voxels = new();
    public MeshInstance3D mesh_instance;

    public Chunk(Vector3I chunk_position, FastNoiseLite noise)
    {
        this.chunk_position = chunk_position;
        world_position = chunk_position * Global.CHUNK_SIZE;
        world_position.Y = 0;

        voxels = GenerateVoxels(noise);
        mesh_instance = GenerateMesh();
    }

    private Hashtable GenerateVoxels(FastNoiseLite noise)
    {
        Hashtable voxels = new();
        Vector3I chunk_world_position = chunk_position * Global.CHUNK_SIZE;

        for (int x = 0; x < Global.CHUNK_SIZE; x++)
        {
            for (int z = 0; z < Global.CHUNK_SIZE; z++)
            {
                float y_noise = noise.GetNoise2D(chunk_world_position.X + x, chunk_world_position.Z + z);
                int y = (int)System.Math.Floor(y_noise * 10);

                Vector3I position = new(x, y, z);
                Voxel voxel = new(position, 0, VoxelMaterial.BASE);
                voxels.Add(position, voxel);
            }
        }

        return voxels;
    }

    private MeshInstance3D GenerateMesh()
    {
        ArrayMesh mesh = new();

        foreach (Voxel voxel in voxels.Values)
        {
            var adjacents = GetAdjacents(voxel);

            if (adjacents.All((adj) => adj)) continue;

            var vertices = GenerateVoxelMesh(voxel, adjacents);
            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, vertices);
        }

        MeshInstance3D mesh_instance = new()
        {
            Mesh = mesh
        };

        return mesh_instance;
    }

    private bool[] GetAdjacents(Voxel voxel)
    {
        // top, bottom, right, left, front, back
        bool[] adjacents = new bool[6];

        int x = voxel.position.X;
        int y = voxel.position.Y;
        int z = voxel.position.Z;

        adjacents[0] = voxels.ContainsKey(new Vector3I(x, y + 1, z));
        adjacents[1] = voxels.ContainsKey(new Vector3I(x, y - 1, z));
        adjacents[2] = voxels.ContainsKey(new Vector3I(x + 1, y, z));
        adjacents[3] = voxels.ContainsKey(new Vector3I(x - 1, y, z));
        adjacents[4] = voxels.ContainsKey(new Vector3I(x, y, z + 1));
        adjacents[5] = voxels.ContainsKey(new Vector3I(x, y, z - 1));

        return adjacents;
    }

    private Array GenerateVoxelMesh(Voxel voxel, bool[] adjacents)
    {
        var mesh_data = new Array();
        mesh_data.Resize((int)Mesh.ArrayType.Max);

        float x = voxel.position.X;
        float y = voxel.position.Y;
        float z = voxel.position.Z;
        mesh_data[(int)Mesh.ArrayType.Vertex] = new Vector3[] {
            voxel.position,           // * 0   0   0  0
            new(x + 1, y, z),         // * 1   1   0  0
            new(x, y, z + 1),         // * 2   0   0  1
            new(x + 1, y, z + 1),     // * 3   1   0  1

            new(x, y - 1, z),         // * 4   0  -1  0
            new(x + 1, y - 1, z),     // * 5   1  -1  0
            new(x, y - 1, z + 1),     // * 6   0  -1  1
            new(x + 1, y - 1, z + 1), // * 7   1  -1  1
        };

        Array<int> indices = new();

        if (!adjacents[0]) indices.AddRange(new int[] { 0, 1, 2, 3, 2, 1 }); // top
        if (!adjacents[1]) indices.AddRange(new int[] { 6, 7, 4, 7, 5, 4 }); // bottom
        if (!adjacents[2]) indices.AddRange(new int[] { 0, 2, 6, 6, 4, 0 }); // left
        if (!adjacents[3]) indices.AddRange(new int[] { 1, 5, 7, 7, 3, 1 }); // right
        if (!adjacents[4]) indices.AddRange(new int[] { 3, 7, 6, 6, 2, 3 }); // front
        if (!adjacents[5]) indices.AddRange(new int[] { 5, 1, 0, 0, 4, 5 }); // back

        mesh_data[(int)Mesh.ArrayType.Index] = indices.ToArray();

        return mesh_data;
    }
}