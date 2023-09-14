using Godot;

public partial class Player : CharacterBody3D
{
    [Export]
    public double gravity = 9.81;
    [Export]
    private int speed = 5;
    [Export]
    private int jump_speed = 5;
    [Export]
    public double mouse_sensitivity = 0.002;

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        velocity.Y += (float)-gravity * (float)delta;
        var input = Input.GetVector("left", "right", "forward", "back");
        var movement_dir = Transform.Basis * new Vector3(input.X, 0, input.Y);
        velocity.X = movement_dir.X * speed;
        velocity.Z = movement_dir.Z * speed;

        Velocity = velocity;

        MoveAndSlide();
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
            velocity.Y = jump_speed;
    }

}