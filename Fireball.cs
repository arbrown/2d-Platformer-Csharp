using Godot;
using System;

public class Fireball : KinematicBody2D
{
    private Vector2 velocity;
    public int Direction = 1;
    private const float SPEED = 800;
    private const float GRAVITY = 22;
    private const float BOUNCE = -300;


    public Sprite Sprite => GetNode<Sprite>("Sprite");

    public AudioStreamPlayer AudioStreamPlayer => GetNode<AudioStreamPlayer>("AudioStreamPlayer");

    public override void _Ready()
    {
        velocity.x = SPEED * Direction;
    }

    public override void _PhysicsProcess(float delta)
    {
        velocity.y += GRAVITY;
        Sprite.RotationDegrees += 25 * Direction;

        if (IsOnWall())
        {
            QueueFree();
        }

        if (IsOnFloor())
        {
            velocity.y = BOUNCE;
        }

        velocity = MoveAndSlide(velocity, Vector2.Up);
    }

    private void _on_VisibilityNotifier2D_screen_exited()
    {
        QueueFree();
    }

    private void _on_Timer_timeout()
    {
        AudioStreamPlayer.Play();
    }
}