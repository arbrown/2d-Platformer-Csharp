using Godot;
using System;

public class Steve : KinematicBody2D
{
	const int SPEED = 180;
	const int GRAVITY = 35;
	const int JUMPFORCE = -1100;
    
    private Vector2 velocity;
	public override void _PhysicsProcess(float delta) {
		if (Input.IsActionPressed("right")) {
			velocity.x = SPEED;
		}
		else if (Input.IsActionPressed("left")) {
			velocity.x = -SPEED;
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor()) {
			velocity.y = JUMPFORCE;
		}

		velocity.y += 30;

		velocity = MoveAndSlide(velocity, Vector2.Up);

		velocity.x = Mathf.Lerp(velocity.x, 0, 0.2f); // Need to specify that the final parameter is a float
	}
}
