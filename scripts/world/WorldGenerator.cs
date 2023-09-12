using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public partial class WorldGenerator : Node
{
    [Export]
    private Node3D player;

    private Array<Vector3I> render_distance_offsets = new();

    private Dictionary<Vector3I, Chunk> chunks = new();
    private Array<Vector3I> render_chunks = new();
    private Array<Chunk> ready_chunks = new();

    private FastNoiseLite noise = new FastNoiseLite();


    public override void _Ready()
    {
        render_distance_offsets = GenerateRenderDistanceOffsets(Global.RENDER_DISTANCE);
    }

    public async override void _Process(double _delta)
    {
        Vector3I player_current_chunk = Global.WorldToChunkCoordinates(player.Position);
        render_chunks = GetChunksInRenderDistance(player_current_chunk);

        Task chunk_generation = Task.Run(() =>
        {
            foreach (Vector3I chunk_pos in render_chunks)
            {
                if (!chunks.ContainsKey(chunk_pos))
                {
                    Chunk chunk = new(chunk_pos, noise);
                    ready_chunks.Add(chunk);
                }
            }
        });

        await chunk_generation;
        foreach (Chunk ready_chunk in ready_chunks)
        {
            chunks[ready_chunk.chunk_position] = ready_chunk;
        }
        ready_chunks.Clear();
    }

    private Array<Vector3I> GetChunksInRenderDistance(Vector3I player_current_chunk)
    {
        Array<Vector3I> positions = new();

        foreach (Vector3I offset in render_distance_offsets) positions.Add(player_current_chunk + offset);

        return positions;
    }

    public Array<Vector3I> GenerateRenderDistanceOffsets(int render_distance)
    {
        Array<Vector3I> offsets = new();
        int render_distance_total = render_distance * 2 + 1;

        for (int x = 0; x < render_distance_total; x++)
        {
            for (int z = 0; z < render_distance_total; z++)
            {
                offsets.Add(new Vector3I(x - render_distance, 0, z - render_distance));
            }
        }

        return offsets;
    }
}