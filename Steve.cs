using Godot;
using System;

public class Steve : KinematicBody2D
{
	const int SPEED = 270;
	const int GRAVITY = 35;
	const int JUMPFORCE = -1100;

	private int coins = 0;
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
		velocity.y += (float)(JUMPFORCE * 0.7);
	}

	public void Ouch(float posx)
	{
		this.Modulate = new Color(1F, 0.3F, 0.3F, 0.3F);
		velocity.y = (float)(JUMPFORCE * 0.5);
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

