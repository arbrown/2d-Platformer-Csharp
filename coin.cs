using Godot;
using System;

public class coin : Area2D
{

    [Signal]
    public delegate void CoinCollected();

    private void _on_coin_body_entered(object body)
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("bounce");
        EmitSignal("CoinCollected");

        SetCollisionMaskBit(0, false);

        GetNode<AudioStreamPlayer>("SoundCoinCollect").Play();
    }
    private void _on_AnimationPlayer_animation_finished(String anim_name)
    {
        QueueFree();
    }
}

