using Godot;

public partial class Slot : TextureRect
{
    public int index;
    public int material;

    public override void _Ready()
    {
        UpdateTexture();
    }

    public void UpdateTexture()
    {
        // Texture = TextureLoader.GetBlockTexture("stone");
    }
}