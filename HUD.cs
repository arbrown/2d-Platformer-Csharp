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


	private void _on_CoinCollected()
	{
		coins++;
		_Ready();
		if (coins == 3)
		{
			GetTree().ChangeScene("res://YouWin.tscn");
		}
	}


}
