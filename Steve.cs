using Godot;
using System;

public class Steve : KinematicBody2D
{
	const int SPEED = 180;
	const int GRAVITY = 35;
	const int JUMPFORCE = -1100;

	private Vector2 velocity;

	public AnimatedSprite Sprite { get => GetNode<AnimatedSprite>("Sprite"); }
	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsActionPressed("right"))
		{
			velocity.x = SPEED;
			Sprite.Play("walk");
			Sprite.FlipH = false;
		}
		else if (Input.IsActionPressed("left"))
		{
			velocity.x = -SPEED;
			Sprite.Play("walk");
			Sprite.FlipH = true;
		}
		else
		{
			Sprite.Play("idle");

		}
		if (!IsOnFloor())
		{
			Sprite.Play("air");
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.y = JUMPFORCE;
		}

		velocity.y += 30;

		velocity = MoveAndSlide(velocity, Vector2.Up);

		velocity.x = Mathf.Lerp(velocity.x, 0, 0.2f); // Need to specify that the final parameter is a float
	}
}
