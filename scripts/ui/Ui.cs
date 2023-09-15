using Godot;

public partial class UI : Control
{
    [Export]
    Control debug_control;
    [Export]
    Node3D world;

    Label fps_label;
    Label player_pos_label;
    Label player_chunk_label;
    Label vp_drawing_mode_label;
    Label camera_label;

    public override void _Ready()
    {

        var win_width = ProjectSettings.GetSetting("display/window/size/viewport_width");
        var win_height = ProjectSettings.GetSetting("display/window/size/viewport_height");

        Scale = new Vector2((float)win_width, (float)win_height) / Size;

        fps_label = debug_control.GetNode<Label>("fps");
        player_pos_label = debug_control.GetNode<Label>("player_pos");
        player_chunk_label = debug_control.GetNode<Label>("player_chunk");
        vp_drawing_mode_label = debug_control.GetNode<Label>("vp_drawing_mode");
        camera_label = debug_control.GetNode<Label>("camera");
    }

    public override void _Process(double _delta)
    {
        fps_label.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();
        vp_drawing_mode_label.Text = "DRAW MODE: " + GetViewport().DebugDraw;

        player_pos_label.Text = "POS: " + world.GetNode<Player>("player").Position;
        player_chunk_label.Text = "CHUNK: " + world.GetNode<World>("generator").player_chunk;
        var camera_rotation = world.GetNode<Player>("player/head").Rotation;


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

        camera_label.Text = "CAMERA: " + camera_rotation + " FACING: " + facing;
    }
}