using System;
using Godot;

public class StationInfo : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        RectPosition = GetTree().Root.FindChildByType<Camera>().UnprojectPosition(this.GetParent<Station>().GetGlobalLocation() + new Vector3(0, 0, -0.45f));
    }
}
