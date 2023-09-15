using Godot;

public partial class Ui : CanvasLayer
{
    [Export]
    public Node3D world;

    Control debug_control;

    Label fps_label;
    Label player_pos_label;
    Label player_chunk_label;
    Label vp_drawing_mode_label;
    Label camera_label;

    Player player;

    public override void _Ready()
    {
        debug_control = GetNode<Control>("debug");

        fps_label = debug_control.GetNode<Label>("fps");
        player_pos_label = debug_control.GetNode<Label>("player_pos");
        player_chunk_label = debug_control.GetNode<Label>("player_chunk");
        vp_drawing_mode_label = debug_control.GetNode<Label>("vp_drawing_mode");
        camera_label = debug_control.GetNode<Label>("camera");

        player = world.GetNode<Player>("player");
    }

    public override void _Process(double _delta)
    {
        fps_label.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();
        vp_drawing_mode_label.Text = "DRAW MODE: " + GetViewport().DebugDraw;

        player_pos_label.Text = "POS: " + player.Position;
        player_chunk_label.Text = "CHUNK: " + world.GetNode<World>("generator").player_chunk;

        Vector3 camera_rotation = player.GetNode<Node3D>("head").Rotation;

        string facing = "north";
        float ry = camera_rotation.Y;

        if (ry < 0.5 && ry > -1)
            facing = "north";
        else if (ry < -1 && ry > -2.5)
            facing = "east";
        else if (ry < -2.5 || ry > 2.5)
            facing = "south";
        else if (ry < 2.5 && ry > 1)
            facing = "ouest";

        camera_label.Text = "CAMERA: " + camera_rotation.ToString() + " FACING: " + facing.ToString();
    }
}