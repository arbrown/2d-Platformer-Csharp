using Godot;
using System;

public class Enemy : KinematicBody2D
{
	[Export]
	private bool detectsCliffs = true;
	private Vector2 velocity;

	[Export]
	private int direction = -1;

	private int speed = 50;

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

			if (detectsCliffs)
			{
				Modulate = new Color(1.2F, 0.5F, 1);
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

		velocity.x = direction * speed;

		velocity = MoveAndSlide(velocity, Vector2.Up);

	}

	private void _on_TopChecker_body_entered(object body)
	{
		speed = 0;
		// A little less careful with null checking  :)
		GetNode<AnimatedSprite>("AnimatedSprite").Play("squashed");
		SetCollisionLayerBit(4, false);
		SetCollisionMaskBit(0, false);
		GetNode<Area2D>("TopChecker").SetCollisionLayerBit(4, false);
		GetNode<Area2D>("TopChecker").SetCollisionMaskBit(0, false);
		GetNode<Area2D>("SidesChecker").SetCollisionLayerBit(4, false);
		GetNode<Area2D>("SidesChecker").SetCollisionMaskBit(0, false);
		GetNode<Timer>("Timer").Start();
		var steve = body as Steve;
		if (steve != null)
		{
			steve.Bounce();
		}

		GetNode<AudioStreamPlayer>("SoundSquash").Play();

	}

	private void _on_SidesChecker_body_entered(object body)
	{
		GD.Print($"Ouch {body}");
		// GetTree().ChangeScene("res://Level1.tscn");
		var steve = body as Steve;
		if (steve != null)
		{
			steve.Ouch(Position.x);
		}
	}

	private void _on_Timer_timeout()
	{
		QueueFree();
	}


}
