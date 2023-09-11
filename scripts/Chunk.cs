using System;
using Godot;

public partial class Chunk : StaticBody3D
{
    public static StaticBody3D GenerateChunk(Vector3 chunk_position, FastNoiseLite noise)
    {
        StaticBody3D chunk = new();
        Vector3 chunk_world_position = Global.ChunkToWorldCoordinates(chunk_position);
        chunk.Position = new Vector3(chunk_world_position.X, 0, chunk_world_position.Z);

        for (int x = 0; x < Global.CHUNK_SIZE; x++)
        {
            for (int z = 0; z < Global.CHUNK_SIZE; z++)
            {
                float y = (float)Math.Floor(noise.GetNoise2D(chunk_world_position.X + x, chunk_world_position.Z + z) * 10);
                BaseMaterial3D material = new Material() as BaseMaterial3D;

                MeshInstance3D mesh = new()
                {
                    Position = new Vector3(x, y, z),
                    MaterialOverride = material,
                    Mesh = new BoxMesh()

                };

                chunk.AddChild(mesh);
            }
        }

        return chunk;
    }
}