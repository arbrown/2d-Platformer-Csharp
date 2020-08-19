using Godot;
using System;

public class HUD : CanvasLayer
{
	private int coins = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var coinsLabel = GetNodeOrNull<Label>("Coins");

		if (coinsLabel != null)
		{
			coinsLabel.Text = $"{coins}";
		}

	}

	public override void _PhysicsProcess(float delta)
	{
		if (coins == 3)
		{
			GetTree().ChangeScene("res://Level1.tscn");
		}
	}

	private void _on_CoinCollected()
	{
		coins++;
		_Ready();
	}


}
