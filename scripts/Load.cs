using Godot;

public partial class Load : Node2D
{
    Control control;

    public override void _Ready()
    {
        control = GetNode<Control>("control");
        var win_width = ProjectSettings.GetSetting("display/window/size/viewport_width");
        var win_height = ProjectSettings.GetSetting("display/window/size/viewport_height");

        control.Size = new Vector2((float)win_width, (float)win_height);

        TextureLoader.LoadTextures();
        GetTree().ChangeSceneToFile("res://scenes/game.tscn");
    }
}