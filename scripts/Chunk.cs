using System;
using System.Collections.Generic;
using Godot;

public partial class Chunk : StaticBody3D
{
    public static StaticBody3D GenerateChunk(Vector3 chunk_position, FastNoiseLite noise)
    {
        StaticBody3D chunk = new();
        Vector3 chunk_world_position = Global.ChunkToWorldCoordinates(chunk_position);
        chunk.Position = new Vector3(chunk_world_position.X, 0, chunk_world_position.Z);

        List<Vector3> voxels_positions = GenerateVoxelsPositions(chunk_world_position, noise);
        // List<Node3D> voxels = GenerateVoxels(voxels_positions);

        MeshInstance3D mesh = ConstructMesh(voxels_positions);

        chunk.AddChild(mesh);

        return chunk;
    }

    private static MeshInstance3D ConstructMesh(List<Vector3> voxels_positions)
    {
        ArrayMesh array_mesh = new();

        foreach (Vector3 position in voxels_positions)
        {
            List<bool> adjacents = GetAdjacentVoxels(position, voxels_positions);
            var (array, no_vertices) = GenerateVoxelMesh(position, adjacents);
            if (no_vertices) continue;
            array_mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, array);
        }

        MeshInstance3D mesh_instance_3d = new()
        {
            Mesh = array_mesh
        };

        return mesh_instance_3d;
    }

    private static List<Vector3> GenerateVoxelsPositions(Vector3 chunk_world_position, FastNoiseLite noise)
    {
        List<Vector3> positions = new();
        for (int x = 0; x < Global.CHUNK_SIZE; x++)
        {
            for (int z = 0; z < Global.CHUNK_SIZE; z++)
            {
                float y = (float)Math.Floor(noise.GetNoise2D(chunk_world_position.X + x, chunk_world_position.Z + z) * 10);
                positions.Add(new Vector3(x, y, z));
            }
        }
        return positions;
    }

    private static List<bool> GetAdjacentVoxels(Vector3 position, List<Vector3> voxels_position)
    {
        // top, bottom, left, right, front, back
        List<bool> adjacents = new() { false, false, false, false, false, false };

        float x = position.X;
        float y = position.Y;
        float z = position.Z;

        foreach (Vector3 voxel_position in voxels_position)
        {
            if (position == voxel_position) continue;

            float vx = voxel_position.X;
            float vy = voxel_position.Y;
            float vz = voxel_position.Z;

            if (y + 1 == vy && z == vz && x == vx) adjacents[0] = true;
            if (y - 1 == vy && z == vz && x == vx) adjacents[1] = true;
            if (x - 1 == vx && z == vz && y == vy) adjacents[2] = true;
            if (x + 1 == vx && z == vz && y == vy) adjacents[3] = true;
            if (z + 1 == vz && y == vy && x == vx) adjacents[4] = true;
            if (z - 1 == vz && y == vy && x == vx) adjacents[5] = true;
        }


        return adjacents;
    }

    private static (Godot.Collections.Array, bool) GenerateVoxelMesh(Vector3 position, List<bool> adjacent)
    {
        var mesh_data = new Godot.Collections.Array();
        mesh_data.Resize((int)Mesh.ArrayType.Max);

        float x = position.X;
        float y = position.Y;
        float z = position.Z;

        mesh_data[(int)Mesh.ArrayType.Vertex] = new Vector3[] {
            position,                 // * 0   0   0  0
            new(x + 1, y, z),         // * 1   1   0  0
            new(x, y, z + 1),         // * 2   0   0  1
            new(x + 1, y, z + 1),     // * 3   1   0  1

            new(x, y - 1, z),         // * 4   0  -1  0
            new(x + 1, y - 1, z),     // * 5   1  -1  0
            new(x, y - 1, z + 1),     // * 6   0  -1  1
            new(x + 1, y - 1, z + 1), // * 7   1  -1  1
        };

        bool no_vertices = adjacent.TrueForAll((is_adjacent) => is_adjacent);
        if (no_vertices) return (null, true);

        List<int> vertices = new();

        if (!adjacent[0]) vertices.AddRange(new int[] { 0, 1, 2, 3, 2, 1 }); // top
        if (!adjacent[1]) vertices.AddRange(new int[] { 6, 7, 4, 7, 5, 4 }); // bottom
        if (!adjacent[2]) vertices.AddRange(new int[] { 0, 2, 6, 6, 4, 0 }); // left
        if (!adjacent[3]) vertices.AddRange(new int[] { 1, 5, 7, 7, 3, 1 }); // right
        if (!adjacent[4]) vertices.AddRange(new int[] { 3, 7, 6, 6, 2, 3 }); // front
        if (!adjacent[5]) vertices.AddRange(new int[] { 5, 1, 0, 0, 4, 5 }); // back

        mesh_data[(int)Mesh.ArrayType.Index] = vertices.ToArray();

        return (mesh_data, no_vertices);
    }
}