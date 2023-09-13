using Godot;
using Godot.Collections;

public partial class WorldGenerator : Node
{
    private const int CHUNK_SIZE = Global.CHUNK_SIZE;

    public static FastNoiseLite noise = new();

    public static Dictionary GenerateChunk(Vector3I chunk_position)
    {
        Dictionary data = new();

        for (int x = 0; x < CHUNK_SIZE; x++)
        {
            for (int z = 0; z < CHUNK_SIZE; z++)
            {
                float y_noise = noise.GetNoise2D(chunk_position.X + x, chunk_position.Z + z);
                int y = (int)System.Math.Floor(y_noise * 10);

                data[new Vector3I(x, y, z)] = VoxelMaterial.BASE;
            }
        }

        return data;
    }
}