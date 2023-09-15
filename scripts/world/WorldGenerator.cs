using Godot;
using Godot.Collections;

public partial class WorldGenerator : Node
{
    private const int CHUNK_SIZE = Global.CHUNK_SIZE;

    public static FastNoiseLite noise = new();

    public static Dictionary<Vector3I, int> GenerateChunk(Vector3I chunk_position)
    {
        Dictionary<Vector3I, int> data = new();
        Vector3I chunk_world_position = chunk_position * Global.CHUNK_SIZE;

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int z = 0; z < CHUNK_SIZE; z++)
            {
                float y_noise = noise.GetNoise2D(chunk_world_position.X + x, chunk_world_position.Z + z);
                int y = (int)System.Math.Floor(y_noise * 10);

                data[new Vector3I(x, y, z)] = VoxelMaterial.STONE;
            }
        }

        return data;
    }
}