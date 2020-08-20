using Godot;
using System;

public class PlayButton : Button
{
	private void _on_PlayButton_pressed()
	{
		GetTree().ChangeScene("res://Level1.tscn");
	}

}


