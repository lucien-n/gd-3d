using Godot;

public partial class Slot : Control
{
    public int index;
    public int material;

    public override void _Ready()
    {
        UpdateTexture();
    }

    public void UpdateTexture()
    {
        var row = material / Global.TEXTURE_SHEET_WIDTH;
        var col = material % Global.TEXTURE_SHEET_WIDTH;
    }
}