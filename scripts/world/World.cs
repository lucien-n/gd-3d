using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public partial class Wrold : Node
{
    [Export]
    public Node3D player;

    [Export]
    public int render_distance = Global.RENDER_DISTANCE;

    private Vector3I _old_player_chunk = new();

    private Dictionary<Vector3I, StaticBody3D> _chunks = new();

    private bool _generating = true;
    private bool _deleting = false;

    public override void _Process(double _delta)
    {
        Vector3I player_chunk = Global.WorldToChunkCoordinates(new Vector3(player.Position.Z, 0, player.Position.Z));

        if (_deleting || player_chunk != _old_player_chunk)
        {
            _DeleteFarChunks(player_chunk);
            _generating = true;
        }

        if (!_generating) return;

        for (int x = 0; x < (player_chunk.X + (render_distance * 2) + 1); x++)
        {
            for (int z = 0; z < (player_chunk.X + (render_distance * 2) + 1); z++)
            {
                Vector3I chunk_pos = new(x, 0, z);

                if (!_IsInRenderDistance(chunk_pos, player_chunk, render_distance)) continue;

                if (_chunks.ContainsKey(chunk_pos)) continue;

                Chunk chunk = new();
                chunk.chunk_position = chunk_pos;
                _chunks.Add(chunk_pos, chunk);
                AddChild(chunk);
            }
        }

        _generating = false;
    }

    private void _DeleteFarChunks(Vector3I player_chunk)
    {
        _old_player_chunk = player_chunk;

        foreach (Vector3I chunk_position in _chunks.Keys)
        {
            if (!_IsInRenderDistance(chunk_position, player_chunk, render_distance))
            {
                _chunks[chunk_position].QueueFree();
                _chunks.Remove(chunk_position);
            }
        }

        _deleting = false;
    }

    private bool _IsInRenderDistance(Vector3I chunk_pos, Vector3I player_chunk, int render_distance)
    {
        return chunk_pos.X > player_chunk.X - render_distance + 1 && chunk_pos.X < player_chunk.X + render_distance + 1 &&
                     chunk_pos.Z > player_chunk.Z - render_distance + 1 && chunk_pos.Z < player_chunk.Z + render_distance + 1;

    }
}