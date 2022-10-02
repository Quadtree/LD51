using System;
using Godot;

public class HardLevelButton : Button
{
    public override void _Ready()
    {
        this.Connect("pressed", this, nameof(OnPressed));
    }

    void OnPressed()
    {
        GetTree().ChangeScene("res://maps/Default.tscn");
    }
}
