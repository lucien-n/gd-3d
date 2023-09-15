using Godot;

public partial class Slot : Control
{
    public int index;
    public int material;

    TextureRect texture_rect;

    public override void _Ready()
    {
        texture_rect = GetNode<TextureRect>("texture_rect");
        UpdateTexture();
    }

    public void UpdateTexture()
    {
        texture_rect.Texture = TextureLoader.GetBlockTexture("stone");
    }
}