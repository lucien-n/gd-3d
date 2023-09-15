using System;
using Godot;

public partial class Global : Node
{

    public const int CHUNK_SIZE = 16;
    public const int RENDER_DISTANCE = 3;
    public const int PLAYER_REACH = 4;
    public const int TEXTURE_SHEET_WIDTH = 8;


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

    public static Texture2D GetBlockTexture(int block_id)
    {
        var uv = Chunk.CalculateBlockUvs(block_id);

        Texture2D sheet = GD.Load<CompressedTexture2D>("res://assets/textures/blocks.png");
        Control item = new Control();

        sheet.DrawRectRegion(item, new Rect2(Vector2.Zero, new Vector2(16, 16)), new Rect2(uv[0] * 512, Vector2.One * 64));

        return;
    }
}