using Godot;
using System;

public class coin : Area2D
{

	private void _on_coin_body_entered(object body)
	{
		GetNode<AnimationPlayer>("AnimationPlayer").Play("bounce");

		var steve = body as Steve;

		if (steve != null)
		{
			steve.AddCoin();
		}
	}
	private void _on_AnimationPlayer_animation_finished(String anim_name)
	{
		QueueFree();
	}
}

