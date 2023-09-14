using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public partial class World : Node
{
    [Export]
    public Node3D player;

    [Export]
    public int render_distance = Global.RENDER_DISTANCE;

    const int CHUNK_END_SIZE = Global.CHUNK_SIZE - 1;

    private Vector3I _old_player_chunk = new();

    private static Dictionary<Vector3I, Chunk> _chunks = new();
    private Array<Vector3I> unready_chunks = new();

    private Task _task;
    private bool _generating = true;
    private bool _deleting = false;

    public override void _Process(double _delta)
    {
        Vector3I player_chunk = Global.WorldToChunkCoordinates(new Vector3(player.Position.X, 0, player.Position.Z));

        if (_deleting || player_chunk != _old_player_chunk)
        {
            DeleteFarChunks(player_chunk);
            _generating = true;
        }

        if (!_generating) return;

        for (int x = 0; x < (player_chunk.X + (render_distance * 2) + 1); x++)
        {
            for (int z = 0; z < (player_chunk.Z + (render_distance * 2) + 1); z++)
            {
                Vector3I chunk_pos = new(x - render_distance, 0, z - render_distance);

                if (!IsInRenderDistance(chunk_pos, player_chunk, render_distance)) continue;

                if (_chunks.ContainsKey(chunk_pos) || unready_chunks.Contains(chunk_pos)) continue;

                Chunk chunk = new()
                {
                    chunk_position = chunk_pos
                };
                _chunks.Add(chunk_pos, chunk);
                AddChild(chunk);
            }
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
            Chunk chunk = _chunks[chunk_position];
            Vector3I sub_position = PosModVI(block_global_position, Global.CHUNK_SIZE);
            if (chunk.data.ContainsKey(sub_position)) return chunk.data[sub_position];
        }

        return VoxelMaterial.AIR;
    }

    public static void SetBlockGlobalPosition(Vector3I block_global_position, int block_id)
    {
        Vector3I chunk_position = block_global_position / Global.CHUNK_SIZE;
        Chunk chunk = _chunks[chunk_position];
        Vector3I sub_position = PosModVI(block_global_position, Global.CHUNK_SIZE);

        if (block_id == 0) chunk.data.Remove(sub_position);
        else chunk.data[sub_position] = block_id;

        chunk.Regenerate();

        if (Chunk.IsBlockTransparent(block_id))
        {
            if (sub_position.X == 0)
                _chunks[chunk_position + Vector3I.Left].Regenerate();
            else if (sub_position.X == CHUNK_END_SIZE)
                _chunks[chunk_position + Vector3I.Right].Regenerate();
            if (sub_position.Z == 0)
                _chunks[chunk_position + Vector3I.Forward].Regenerate();
            else if (sub_position.Z == CHUNK_END_SIZE)
                _chunks[chunk_position + Vector3I.Back].Regenerate();
            if (sub_position.Y == 0)
                _chunks[chunk_position + Vector3I.Down].Regenerate();
            else if (sub_position.Y == CHUNK_END_SIZE)
                _chunks[chunk_position + Vector3I.Up].Regenerate();
        }
    }

    public static bool IsBlockFloating(Vector3I block_global_position)
    {
        bool is_floating = GetBlockGlobalPosition(block_global_position - Vector3I.Left) == VoxelMaterial.AIR &&
                GetBlockGlobalPosition(block_global_position - Vector3I.Right) == VoxelMaterial.AIR &&
                GetBlockGlobalPosition(block_global_position - Vector3I.Up) == VoxelMaterial.AIR &&
                GetBlockGlobalPosition(block_global_position - Vector3I.Down) == VoxelMaterial.AIR &&
                GetBlockGlobalPosition(block_global_position - Vector3I.Forward) == VoxelMaterial.AIR &&
                GetBlockGlobalPosition(block_global_position - Vector3I.Back) == VoxelMaterial.AIR;

        return is_floating;
    }

    public static bool PlaceBlockAsPlayer(Vector3I block_global_position, int block_id)
    {
        if (GetBlockGlobalPosition(block_global_position) != VoxelMaterial.AIR || IsBlockFloating(block_global_position)) return false;

        SetBlockGlobalPosition(block_global_position, block_id);

        return true;
    }

    public static bool BreakBlockAsPlayer(Vector3I block_global_position)
    {
        if (GetBlockGlobalPosition(block_global_position) == VoxelMaterial.AIR) return false;

        SetBlockGlobalPosition(block_global_position, VoxelMaterial.AIR);

        return true;
    }

    private static Vector3I PosModVI(Vector3I value, int modulo)
    {
        return (Vector3I)((Vector3)value).PosMod(modulo);
    }
}