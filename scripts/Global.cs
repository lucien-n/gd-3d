using System;
using Godot;

public partial class Global : Node
{

    public const float CHUNK_SIZE = 16;
    public const float RENDER_DISTANCE = 2;

    public static Vector3 WorldToChunkCoordinates(Vector3 world_position)
    {
        Vector3 chunk_position = Vector3.Zero;

        chunk_position.X = (float)Math.Floor(world_position.X / CHUNK_SIZE);
        chunk_position.Y = (float)Math.Floor(world_position.Y / CHUNK_SIZE);
        chunk_position.Z = (float)Math.Floor(world_position.Z / CHUNK_SIZE);

        return chunk_position;
    }

    public static Vector3 ChunkToWorldCoordinates(Vector3 chunk_position)
    {
        Vector3 world_position = Vector3.Zero;

        world_position.X = chunk_position.X * CHUNK_SIZE;
        world_position.Y = chunk_position.Y * CHUNK_SIZE;
        world_position.Z = chunk_position.Z * CHUNK_SIZE;

        return world_position;
    }
}