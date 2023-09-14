using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public partial class World : Node
{
    [Export]
    public Node3D player;

    [Export]
    public int render_distance = Global.RENDER_DISTANCE;

    private Vector3I _old_player_chunk = new();

    private static Dictionary<Vector3I, StaticBody3D> _chunks = new();
    private Array<Vector3I> unready_chunks = new();

    private Task _task;
    private bool _generating = true;
    private bool _deleting = false;

    public async override void _Process(double _delta)
    {
        Vector3I player_chunk = Global.WorldToChunkCoordinates(new Vector3(player.Position.X, 0, player.Position.Z));

        if (_deleting || player_chunk != _old_player_chunk)
        {
            DeleteFarChunks(player_chunk);
            _generating = true;
        }

        if (!_generating) return;

        Array<Chunk> ready_chunks = new();

        _task = Task.Run(() =>
        {

            for (int x = 0; x < (player_chunk.X + (render_distance * 2) + 1); x++)
            {
                for (int z = 0; z < (player_chunk.Z + (render_distance * 2) + 1); z++)
                {
                    Vector3I chunk_pos = new(x - render_distance, 0, z - render_distance);

                    if (!IsInRenderDistance(chunk_pos, player_chunk, render_distance)) continue;

                    if (_chunks.ContainsKey(chunk_pos) || unready_chunks.Contains(chunk_pos)) continue;

                    unready_chunks.Add(chunk_pos);
                    Chunk chunk = new()
                    {
                        chunk_position = chunk_pos
                    };
                    ready_chunks.Add(chunk);
                }
            }
        });

        await _task;

        var new_chunks = ready_chunks;
        foreach (Chunk ready_chunk in new_chunks)
        {
            unready_chunks.Remove(ready_chunk.chunk_position);
            _chunks.Add(ready_chunk.chunk_position, ready_chunk);
            AddChild(ready_chunk);
        }

        _generating = false;
    }

    private void DeleteFarChunks(Vector3I player_chunk)
    {
        _old_player_chunk = player_chunk;

        foreach (Vector3I chunk_position in _chunks.Keys)
        {
            if (!IsInRenderDistance(chunk_position, player_chunk, render_distance))
            {
                _chunks[chunk_position].QueueFree();
                _chunks.Remove(chunk_position);
            }
        }

        _deleting = false;
    }

    private bool IsInRenderDistance(Vector3I chunk_pos, Vector3I player_chunk, int render_distance)
    {
        return chunk_pos.X >= player_chunk.X - render_distance + 1 && chunk_pos.X <= player_chunk.X + render_distance + 1 &&
                     chunk_pos.Z >= player_chunk.Z - render_distance + 1 && chunk_pos.Z <= player_chunk.Z + render_distance + 1;

    }

    public static int GetBlockGlobalPosition(Vector3I block_global_position)
    {
        Vector3I chunk_position = block_global_position / Global.CHUNK_SIZE;
        if (_chunks.ContainsKey(chunk_position))
        {
            Chunk chunk = (Chunk)_chunks[chunk_position];
            Vector3I sub_position = PosModVI(block_global_position, Global.CHUNK_SIZE);
            if (chunk.data.ContainsKey(sub_position)) return chunk.data[sub_position];
        }

        return VoxelMaterial.AIR;
    }

    public static void SetBlockGlobalPosition(Vector3I block_global_position, int block_id)
    {
        Vector3I chunk_position = block_global_position / Global.CHUNK_SIZE;
        Chunk chunk = (Chunk)_chunks[chunk_position];
        Vector3I sub_position = PosModVI(block_global_position, Global.CHUNK_SIZE);

        if (block_id == 0) chunk.data.Remove(sub_position);
        else chunk.data[sub_position] = block_id;

        chunk.Regenerate();

        // TODO: Regenerate neighboring chunks if block is transparent
    }

    private static Vector3I PosModVI(Vector3I value, int modulo)
    {
        return (Vector3I)((Vector3)value).PosMod(modulo);
    }
}