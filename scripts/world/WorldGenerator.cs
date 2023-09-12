using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class WorldGenerator : Node
{
    [Export]
    private Node3D player;

    private List<Vector3I> render_distance_offsets = new();

    private Dictionary<Vector3I, Chunk> chunks = new();
    private List<Vector3I> render_chunks = new();
    private List<Vector3I> unready_chunks = new();
    private List<Chunk> ready_chunks = new();

    private FastNoiseLite noise = new FastNoiseLite();


    public override void _Ready()
    {
        render_distance_offsets = GenerateRenderDistanceOffsets(Global.RENDER_DISTANCE);
    }

    public override void _Process(double _delta)
    {
        Vector3I player_current_chunk = Global.WorldToChunkCoordinates(new Vector3I((int)player.Position.X, 0, (int)player.Position.Z));
        render_chunks = GetChunksInRenderDistance(player_current_chunk);

        foreach (Vector3I chunk_pos in render_chunks)
        {
            if (!chunks.ContainsKey(chunk_pos) && !unready_chunks.Contains(chunk_pos))
            {
                unready_chunks.Add(chunk_pos);
                Chunk chunk = new(chunk_pos, noise);
                ready_chunks.Add(chunk);
            }
        }

        if (ready_chunks.Count > 0)
        {
            Chunk ready_chunk = ready_chunks[0];
            unready_chunks.Remove(ready_chunk.chunk_position);
            ready_chunks.Remove(ready_chunk);
            chunks[ready_chunk.chunk_position] = ready_chunk;

            ready_chunk.mesh_instance.Position = ready_chunk.world_position;
            AddChild(ready_chunk.mesh_instance);

            GD.Print(GetChildCount() * 256, " surfaces");
        }

    }

    private List<Vector3I> GetChunksInRenderDistance(Vector3I player_current_chunk)
    {
        List<Vector3I> positions = new();

        foreach (Vector3I offset in render_distance_offsets) positions.Add(player_current_chunk + offset);

        return positions;
    }

    public List<Vector3I> GenerateRenderDistanceOffsets(int render_distance)
    {
        List<Vector3I> offsets = new();
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