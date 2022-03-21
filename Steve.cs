using Godot;
using System;

public class Steve : KinematicBody2D
{
	const int SPEED = 270;
	const int RUNSPEED = 400;
	const int GRAVITY = 35;
	const int JUMPFORCE = -1100;
	
	private State state = State.Air;
	private int coins = 0;
	private Vector2 velocity;
	
	private PackedScene fireBallScene = (PackedScene) GD.Load("res://Fireball.tscn");

	public AnimatedSprite Sprite
	{
		get => GetNode<AnimatedSprite>("Sprite");
	}

	public override void _PhysicsProcess(float delta)
	{
		Console.WriteLine(velocity.x);
		switch (state)
		{
			case State.Air:
				if (IsOnFloor())
				{
					state = State.Floor;
					goto case State.Floor; // Any other way to continue processing on this cycle?
				}

				Sprite.Play("air");
				if (Input.IsActionPressed("right"))
				{
					velocity.x = velocity.x < SPEED
						? Mathf.Lerp(velocity.x, SPEED, 0.1f)
						: Mathf.Lerp(velocity.x, SPEED, 0.03f);
					Sprite.FlipH = false;
				}
				else if (Input.IsActionPressed("left"))
				{
					velocity.x = velocity.x > -SPEED // Note the flipped check for -SPEED
						? Mathf.Lerp(velocity.x, -SPEED, 0.1f)
						: Mathf.Lerp(velocity.x, -SPEED, 0.03f);
					Sprite.FlipH = true;
				}
				else
				{
					velocity.x = Mathf.Lerp(velocity.x, 0, 0.2f);
				}

				MoveAndFall();
				Fire();
				break;
			case State.Floor:
				if (!IsOnFloor())
				{
					state = State.Air;
				}

				if (Input.IsActionPressed("right"))
				{
					if (Input.IsActionPressed("run"))
					{
						velocity.x = Mathf.Lerp(velocity.x, RUNSPEED, 0.1f);
						Sprite.SpeedScale = 1.8f;
					}
					else
					{
						velocity.x = Mathf.Lerp(velocity.x, SPEED, 0.1f);
						Sprite.SpeedScale = 1.0f;
					}

					Sprite.Play("walk");
					Sprite.FlipH = false;
				}
				else if (Input.IsActionPressed("left"))
				{
					if (Input.IsActionPressed("run"))
					{
						velocity.x = Mathf.Lerp(velocity.x, -RUNSPEED, 0.1f);
						Sprite.SpeedScale = 1.8f;
					}
					else
					{
						velocity.x = Mathf.Lerp(velocity.x, -SPEED, 0.1f);
						Sprite.SpeedScale = 1.0f;
					}

					Sprite.Play("walk");
					Sprite.FlipH = true;
				}
				else
				{
					Sprite.Play("idle");
					velocity.x = Mathf.Lerp(velocity.x, 0, 0.2f);
				}

				if (Input.IsActionJustPressed("jump"))
				{
					velocity.y = JUMPFORCE;
					GetNode<AudioStreamPlayer>("SoundJump").Play();
				}

				MoveAndFall();
				Fire();
				break;
			case State.Ladder:
				break;
			case State.Wall:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void MoveAndFall()
	{
		velocity.y += 30;
		velocity = MoveAndSlide(velocity, Vector2.Up);
	}

	private void Fire()
	{
		if (Input.IsActionJustPressed("fire"))
		{
			var f = fireBallScene.Instance() as Fireball;
			if (f is Fireball)
			{
				var direction = Sprite.FlipH ? -1 : 1;
				f.Direction = direction;
				GetParent().AddChild(f);
				var pos = f.Position;
				pos.y = Position.y;
				pos.x = Position.x + 25 * direction;
				f.Position = pos;
				
			}

		}
	}

	private void _on_fallzone_body_entered(object body)
	{
		GetTree().ChangeScene("res://GameOver.tscn");
	}

	public void AddCoin()
	{
		coins++;
	}

	public void Bounce()
	{
		velocity.y = (float) (JUMPFORCE * 0.7);
	}

	public void Ouch(float posx)
	{
		this.Modulate = new Color(1F, 0.3F, 0.3F, 0.3F);
		velocity.y = (float) (JUMPFORCE * 0.5);
		if (Position.x < posx)
		{
			velocity.x = -800;
		}
		else if (Position.x > posx)
		{
			velocity.x = 800;
		}

		Input.ActionRelease("left");
		Input.ActionRelease("right");

		GetNode<Timer>("Timer").Start();
	}

	private void _on_Timer_timeout()
	{
		GetTree().ChangeScene("res://GameOver.tscn");
	}
}

internal enum State
{
	Air = 1, // Jumping or falling
	Floor, // Walking or running
	Ladder, // Climbing
	Wall, // Wall-jumping
}
