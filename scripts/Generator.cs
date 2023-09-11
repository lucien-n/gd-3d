using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class Generator : Node
{
    [Export]
    private Node3D _player;

    private Dictionary<Vector3, StaticBody3D> chunks = new();
    private List<Vector3> unready_chunks = new();
    private readonly FastNoiseLite noise = new();

    public override void _Process(double delta)
    {
        UpdateChunks();
    }

    private List<Vector3> GetChunksInRenderDistance(Vector3 from)
    {
        List<Vector3> render_chunks = new();

        for (int x = 0; x < Global.RENDER_DISTANCE * 2 + 1; x++)
        {
            for (int z = 0; z < Global.RENDER_DISTANCE * 2 + 1; z++)
            {
                Vector3 position = new(
                    from.X + x - Global.RENDER_DISTANCE,
                    0,
                    from.Z + z - Global.RENDER_DISTANCE
                );
                render_chunks.Add(position);
            }
        }

        return render_chunks;
    }

    private async void UpdateChunks()
    {
        Vector3 player_chunk_position = Global.WorldToChunkCoordinates(_player.Position);
        List<Vector3> render_chunks = GetChunksInRenderDistance(player_chunk_position);

        List<StaticBody3D> ready_chunks = new();

        Task chunk_generation = Task.Run(() =>
        {
            foreach (Vector3 chunk_position in render_chunks)
            {
                if (!chunks.Keys.Contains(chunk_position) && !unready_chunks.Contains(chunk_position))
                {
                    unready_chunks.Add(chunk_position);
                    StaticBody3D chunk = GenerateChunk(chunk_position);
                    ready_chunks.Add(chunk);
                }
            }
        });

        await chunk_generation;
        foreach (StaticBody3D chunk in ready_chunks)
        {
            AddChild(chunk);
        }
    }

    private StaticBody3D GenerateChunk(Vector3 chunk_position)
    {
        StaticBody3D chunk = Chunk.GenerateChunk(chunk_position, noise);


        chunks.Add(chunk_position, chunk);
        unready_chunks.Remove(chunk_position);

        return chunk;
    }
}