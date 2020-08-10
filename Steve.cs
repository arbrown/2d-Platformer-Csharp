using Godot;
using System;

public class Steve : KinematicBody2D
{
	private Vector2 velocity;
	public override void _PhysicsProcess(float delta) {
		if (Input.IsActionPressed("right")) {
			velocity.x = 100;
		}
		else if (Input.IsActionPressed("left")) {
			velocity.x = -100;
		}
		MoveAndSlide(velocity);
		velocity.x = Mathf.Lerp(velocity.x, 0, 0.2f); // Need to specify that the final parameter is a float
	}
}
