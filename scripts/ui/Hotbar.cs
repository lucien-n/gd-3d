using Godot;
using Godot.Collections;

public partial class Hotbar : Control
{
    private Dictionary<int, Slot> _slots = new();

    private int selected_slot = 0;
    private GridContainer slots_container;
    private TextureRect selected_texture;

    [Export]
    PackedScene slot_scene;

    public override void _Ready()
    {
        selected_texture = GetNode<TextureRect>("selected");
        slots_container = GetNode<GridContainer>("slots");

        for (int i = 0; i < 9; i++)
        {
            Slot slot = slot_scene.Instantiate() as Slot;
            slot.index = i;
            slot.material = Materials.STONE;
            _slots[i] = slot;
            slots_container.AddChild(slot);
        }

        _slots[0].material = Materials.STONE;
        _slots[1].material = Materials.DIRT;
        _slots[2].material = Materials.COBBLESTONE;
        _slots[3].material = Materials.DIAMOND_BLOCK;
        _slots[4].material = Materials.BRICK;
        _slots[5].material = Materials.STONE_BRICK;
        _slots[6].material = Materials.SAND;
        _slots[7].material = Materials.GRAVEL;
        _slots[8].material = Materials.GRASS;


        foreach (Slot slot in _slots.Values)
        {
            slot.UpdateTexture();
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
                if (e.ButtonIndex == MouseButton.WheelDown)
                {
                    selected_slot += 1;
                    if (selected_slot == 9) selected_slot = 0;
                }
                if (e.ButtonIndex == MouseButton.WheelUp)
                {
                    selected_slot -= 1;
                    if (selected_slot == -1) selected_slot = 8;
                }

                Global.PLAYER_HOLDING = _slots[selected_slot].material;
            }
        }
        else if (@event is InputEventKey)
        {
            InputEventKey e = @event as InputEventKey;
            if (e.IsPressed() && (int)e.Keycode > 47 && (int)e.Keycode < 58)
            {
                selected_slot = (int)e.Keycode - 49;
            }
        }
    }
}