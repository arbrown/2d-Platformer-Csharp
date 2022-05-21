using Godot;
using System;

public class LadderTop : StaticBody2D
{
    private bool aboveLadder = false;

    public CollisionShape2D CollisionShape2D => GetNode<CollisionShape2D>("CollisionShape2D");

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionPressed("down") && aboveLadder)
        {
            CollisionShape2D.RotationDegrees = 180;
        }
        else
        {
            CollisionShape2D.RotationDegrees = 0;
        }
    }

    private void _on_AboveChecker_body_entered(object body)
    {
        aboveLadder = true;
    }

    private void _on_AboveChecker_body_exited(object body)
    {
        aboveLadder = false;
    }
}