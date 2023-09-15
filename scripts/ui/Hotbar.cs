using Godot;
using Godot.Collections;

public partial class Hotbar : Control
{
    private Dictionary<int, Slot> _slots = new();
    private int selected_slot = 0;

    private TextureRect hotbar_texture;
    private TextureRect selected_texture;

    public override void _Ready()
    {
        hotbar_texture = GetNode<TextureRect>("hotbar");
        selected_texture = GetNode<TextureRect>("selected");

        for (int i = 0; i < 9; i++)
        {
            Slot slot = new()
            {
                index = i,
                material = VoxelMaterial.STONE
            };
            _slots[i] = slot;
            AddChild(slot);
        }
    }

    public override void _Process(double _delta)
    {
        selected_texture.Position = new Vector2(selected_slot * 40 - 2, -2);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            InputEventMouseButton e = @event as InputEventMouseButton;
            if (e.IsPressed())
            {
                if (e.ButtonIndex == MouseButton.WheelUp)
                {
                    selected_slot += 1;
                    if (selected_slot == 9) selected_slot = 0;
                }
                if (e.ButtonIndex == MouseButton.WheelDown)
                {
                    selected_slot -= 1;
                    if (selected_slot == -1) selected_slot = 8;
                }
            }
        }
    }
}