using Godot;
using System;
using System.Runtime.Serialization;

public class Steve : KinematicBody2D
{
    const int SPEED = 270;
    const int RUNSPEED = 400;
    const int GRAVITY = 35;
    const int JUMPFORCE = -1100;

    private int lastJumpDirection = 0;
    private bool isOnLadder = false;
    private State state = State.Air;
    private int coins = 0;
    private Vector2 velocity;
    private int direction = 1;

    private PackedScene fireBallScene = (PackedScene) GD.Load("res://Fireball.tscn");

    public AnimatedSprite Sprite
    {
        get => GetNode<AnimatedSprite>("Sprite");
    }

    public RayCast2D WallChecker => GetNode<RayCast2D>("WallChecker");

    public override void _PhysicsProcess(float delta)
    {
        Console.WriteLine(IsNearWall());
        switch (state)
        {
            case State.Air:
                if (IsOnFloor() && velocity.y == 0)
                {
                    state = State.Floor;
                    lastJumpDirection = 0;
                    goto case State.Floor; // Any other way to continue processing on this cycle?
                }
                else if (IsNearWall())
                {
                    state = State.Wall;
                    goto case State.Wall;
                }
                else if (ShouldClimbLadder())
                {
                    state = State.Ladder;
                    goto case State.Ladder;
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

                SetDirection();
                MoveAndFall();
                Fire();
                break;
            case State.Floor:
                if (!IsOnFloor())
                {
                    state = State.Air;
                    goto case State.Air;
                }
                else if (ShouldClimbLadder())
                {
                    state = State.Ladder;
                    // Need to skip this goto in order to allow a frame to pass
                    // so that a ladder top's collisionShape can rotate 180 degrees
                    // and let IsOnFloor return the correct answer (false)
                    // goto case State.Ladder;
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

                SetDirection();
                MoveAndFall();
                Fire();
                break;
            case State.Ladder:
                if (!isOnLadder)
                {
                    state = State.Air;
                    goto case State.Air;
                }
                else if (IsOnFloor() && Input.IsActionPressed("down") && velocity.y == 0)
                {
                    state = State.Floor;
                    Input.ActionRelease("down");
                    Input.ActionRelease("up");
                    goto case State.Floor;
                }
                else if (Input.IsActionJustPressed("jump"))
                {
                    Input.ActionRelease("down");
                    Input.ActionRelease("up");
                    velocity.y = JUMPFORCE * 0.7f;
                    state = State.Air;
                    goto case State.Air;
                }

                if (Input.IsActionPressed("down") || Input.IsActionPressed("up") ||
                    Input.IsActionPressed("left") || Input.IsActionPressed("right"))
                {
                    Sprite.Play("climb");
                }
                else
                {
                    Sprite.Stop();
                }

                if (Input.IsActionPressed("up"))
                {
                    velocity.y = -SPEED;
                }
                else if (Input.IsActionPressed("down"))
                {
                    velocity.y = SPEED;
                }
                else
                {
                    velocity.y = Mathf.Lerp(velocity.y, 0, 0.3f);
                }

                if (Input.IsActionPressed("left"))
                {
                    velocity.x = -SPEED / 6;
                }
                else if (Input.IsActionPressed("right"))
                {
                    velocity.x = SPEED / 6;
                }
                else
                {
                    velocity.x = Mathf.Lerp(velocity.x, 0, 0.3f);
                }

                velocity = MoveAndSlide(velocity, Vector2.Up);

                break;

            case State.Wall:
                if (IsOnFloor())
                {
                    state = State.Floor;
                    lastJumpDirection = 0;
                    goto case State.Floor;
                }
                else if (!IsNearWall())
                {
                    state = State.Air;
                    goto case State.Air;
                }

                Sprite.Play("wall");

                if (Input.IsActionPressed("jump") &&
                    ((Input.IsActionPressed("left") && direction == 1)
                     || (Input.IsActionPressed("right") && direction == -1))
                    && lastJumpDirection != direction)
                {
                    lastJumpDirection = direction;
                    velocity.x = 450 * -direction;
                    velocity.y = JUMPFORCE * 0.7f;
                    GetNode<AudioStreamPlayer>("SoundJump").Play();
                    state = State.Air;
                }

                MoveAndFall(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MoveAndFall(bool slowFall = false)
    {
        velocity.y += GRAVITY;
        if (slowFall)
        {
            velocity.y = Mathf.Clamp(velocity.y, JUMPFORCE, 200);
        }

        velocity = MoveAndSlide(velocity, Vector2.Up);
    }

    private bool ShouldClimbLadder()
    {
        return isOnLadder && (Input.IsActionPressed("up") || Input.IsActionPressed("down"));
    }

    private void SetDirection()
    {
        direction = Sprite.FlipH ? -1 : 1;
        WallChecker.RotationDegrees = 90 * -direction;
    }

    private bool IsNearWall()
    {
        if (WallChecker.IsColliding())
        {
            var collider = WallChecker.GetCollider() as Node2D;
            if (collider.IsInGroup("one_way"))
            {
                return false;
            }

            return true;
        }

        return false;
    }

    private void Fire()
    {
        if (Input.IsActionJustPressed("fire") && !IsNearWall())
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

    private void _on_LadderChecker_body_entered(object body)
    {
        isOnLadder = true;
    }

    private void _on_LadderChecker_body_exited(object body)
    {
        isOnLadder = false;
    }
}

internal enum State
{
    Air = 1, // Jumping or falling
    Floor, // Walking or running
    Ladder, // Climbing
    Wall, // Wall-jumping
}