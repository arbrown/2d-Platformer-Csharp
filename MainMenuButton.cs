using Godot;
using System;

public class MainMenuButton : Button
{
	private void _on_MainMenuButton_pressed()
	{
		GetTree().ChangeScene("res://Level1.tscn");
	}
}



