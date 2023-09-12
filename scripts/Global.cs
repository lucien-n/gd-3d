using System;
using Godot;

public partial class Global : Node
{

    public const int CHUNK_SIZE = 16;
    public const int RENDER_DISTANCE = 6;

    public static Vector3I WorldToChunkCoordinates(Vector3 world_position)
    {
        Vector3I chunk_position = Vector3I.Zero;

        chunk_position.X = (int)Math.Floor(world_position.X / CHUNK_SIZE);
        chunk_position.Y = (int)Math.Floor(world_position.Y / CHUNK_SIZE);
        chunk_position.Z = (int)Math.Floor(world_position.Z / CHUNK_SIZE);

        return chunk_position;
    }

    public static Vector3I ChunkToWorldCoordinates(Vector3I chunk_position)
    {
        Vector3I world_position = Vector3I.Zero;

        world_position.X = chunk_position.X * CHUNK_SIZE;
        world_position.Y = chunk_position.Y * CHUNK_SIZE;
        world_position.Z = chunk_position.Z * CHUNK_SIZE;

        return world_position;
    }
}