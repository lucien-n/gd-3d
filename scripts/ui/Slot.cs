using Godot;

public partial class Slot : Panel
{
    public int index;
    public VoxelMaterial material;

    Sprite2D sprite;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("sprite");
        UpdateTexture();
    }

    public void UpdateTexture()
    {
        sprite.Texture = TextureLoader.GetBlockTexture(material.name);
    }
}