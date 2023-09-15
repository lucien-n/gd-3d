using Godot;

public partial class Game : Node3D
{
    Viewport vp;

    public Game()
    {
        RenderingServer.SetDebugGenerateWireframes(true);
    }

    public override void _Ready()
    {
        vp = GetViewport();
        TextureLoader.LoadTextures();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey && Input.IsKeyPressed(Key.P))
            vp.DebugDraw++;
        if (@event is InputEventKey && Input.IsKeyPressed(Key.O))
            vp.DebugDraw--;
    }
}