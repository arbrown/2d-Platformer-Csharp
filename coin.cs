using Godot;
using System;

public class coin : Area2D
{

	private void _on_coin_body_entered(object body)
	{
		QueueFree();
	}
}



