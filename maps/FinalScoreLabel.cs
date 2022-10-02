using System;
using Godot;

public class FinalScoreLabel : Label
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Text = $"Your Score: {Default.Score}\n\n" +
        $"Level Par:\n" +
        $"Bronze Score: {Default.BronzeScore}\n" +
        $"Silver Score: {Default.SilverScore}\n" +
        $"Gold Score: {Default.GoldScore}\n";
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //
    //  }
}
