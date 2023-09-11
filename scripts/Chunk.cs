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
        List<Node3D> voxels = GenerateVoxels(voxels_positions);

        foreach (Node3D voxel in voxels)
        {
            chunk.AddChild(voxel);
        }

        return chunk;
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

    private static List<Node3D> GenerateVoxels(List<Vector3> voxels_positions)
    {
        List<Node3D> voxels = new();

        foreach (Vector3 position in voxels_positions)
        {
            MeshInstance3D mesh = new()
            {
                Position = position,
                MaterialOverride = new Material() as BaseMaterial3D,
                Mesh = new BoxMesh()
            };
            voxels.Add(mesh);
        }

        return voxels;
    }
}