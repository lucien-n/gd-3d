using System;
using Godot;

public partial class Player : CharacterBody3D
{

    [Export]
    float speed = 6.21F;
    [Export]
    const int ACCEL_DEFAULT = 18;
    [Export]
    const int ACCEL_AIR = 1;
    [Export]
    int accel = ACCEL_DEFAULT;

    [Export]
    float gravity = 9.8F;

    [Export]
    float jump = 5F;

    [Export]
    const int cam_accel = 40;
    [Export]
    float mouse_sense = 0.1F;

    Vector3 direction = Vector3.Zero;
    Vector3 velocity = Vector3.Zero;
    Vector3 gravity_vec = Vector3.Zero;
    Vector3 movement = Vector3.Zero;

    Node3D head;
    Camera3D camera;
    RayCast3D raycast;

    public override void _Ready()
    {
        head = GetNode<Node3D>("head");
        camera = GetNode<Camera3D>("head/camera");
        raycast = GetNode<RayCast3D>("head/raycast");
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion)
        {
            InputEventMouseMotion ev = e as InputEventMouseMotion;
            RotateY(Mathf.DegToRad(-ev.Relative.X * mouse_sense));
            head.RotateX(Mathf.DegToRad(-ev.Relative.Y * mouse_sense));
            Vector3 head_rotation = head.Rotation;
            head_rotation.X = Math.Clamp(head.Rotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
            head.Rotation = head_rotation;
        }

        if (e is InputEventMouseButton)
        {
            InputEventMouseButton ev = e as InputEventMouseButton;
            if (!ev.Pressed) return;

            raycast.ForceRaycastUpdate();
            if (!raycast.IsColliding()) return;

            Vector3 position = raycast.GetCollisionPoint();
            Vector3 normal = raycast.GetCollisionNormal();

            if (ev.ButtonIndex == MouseButton.Left)
            {
                Vector3I block_global_position = (Vector3I)(position - normal / 2).Floor();
                GD.Print(block_global_position);
                World.BreakBlockAsPlayer(block_global_position, raycast.GlobalPosition);
            }
            else if (ev.ButtonIndex == MouseButton.Right)
            {
                Vector3I block_global_position = (Vector3I)(position + normal / 2).Floor();
                World.PlaceBlockAsPlayer(block_global_position, raycast.GlobalPosition, Global.PLAYER_HOLDING);
            }
        }
    }

    public override void _Process(double delta)
    {
        if (Engine.GetFramesPerSecond() > Engine.PhysicsTicksPerSecond)
        {
            camera.TopLevel = true;
            camera.Position = camera.GlobalTransform.Origin.Lerp(head.GlobalTransform.Origin, cam_accel * (float)delta);
            Vector3 camera_rotation = camera.Rotation;
            camera_rotation.Y = Rotation.Y;
            camera_rotation.X = head.Rotation.X;
            camera.Rotation = camera_rotation;
        }
        else
        {
            camera.TopLevel = true;
            camera.Position = head.Position;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        direction = Vector3.Zero;
        var h_rot = GlobalTransform.Basis.GetEuler().Y;
        var f_input = Input.GetActionRawStrength("move_backward") - Input.GetActionRawStrength("move_forward");
        var h_input = Input.GetActionRawStrength("move_right") - Input.GetActionRawStrength("move_left");

        direction = new Vector3(h_input, 0, f_input).Rotated(Vector3.Up, h_rot).Normalized();

        if (IsOnFloor())
        {
            accel = ACCEL_DEFAULT;
            gravity_vec = Vector3.Zero;
        }
        else
        {
            accel = ACCEL_AIR;
            gravity_vec += Vector3.Down * gravity * (float)delta;
        }

        if (Input.IsActionJustPressed("jump") && IsOnFloor())
        {
            gravity_vec = Vector3.Up * jump;
        }

        velocity = velocity.Lerp(direction * speed, accel * (float)delta);
        movement = velocity + gravity_vec;
        Velocity = movement;

        MoveAndSlide();
    }

}
