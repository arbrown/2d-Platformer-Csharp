using Godot;
using System;

public class Enemy : KinematicBody2D
{
	[Export]
	private bool detectsCliffs = true;
	private Vector2 velocity;

	[Export]
	private int direction = -1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (direction > 0)
		{
			var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
			if (animatedSprite != null)
			{
				animatedSprite.FlipH = true;
			}

			// GetNode<RayCast2D>("FloorChecker")?.Position.x = GetNode<CollisionShape2D>("CollisionShape2D")?.GetNode<RectangleShape2D>("shape")?.Extents.x * direction;
			var floorChecker = GetNode<RayCast2D>("FloorChecker");
			if (floorChecker != null)
			{
				floorChecker.Enabled = detectsCliffs;

				var pos = floorChecker.Position;
				var rect = GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D;
				pos.x = (float)rect?.Extents.x * direction;
				floorChecker.Position = pos;
			}

		}
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (IsOnWall() || !GetNode<RayCast2D>("FloorChecker").IsColliding() && detectsCliffs && IsOnFloor())
		{
			direction *= -1;
			var animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
			if (animatedSprite != null)
			{
				animatedSprite.FlipH = !animatedSprite.FlipH;
				var floorChecker = GetNode<RayCast2D>("FloorChecker");
				if (floorChecker != null)
				{
					var pos = floorChecker.Position;
					var rect = GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D;
					pos.x = (float)rect?.Extents.x * direction;
					floorChecker.Position = pos;
				}
			}
		}

		velocity.y += 20;

		velocity.x = direction * 50;

		velocity = MoveAndSlide(velocity, Vector2.Up);

	}
}
