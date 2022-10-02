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
        var sta = this.GetParent<Station>();
        var cam = GetTree().Root.FindChildByType<Camera>();
        RectPosition = cam.UnprojectPosition(sta.GetGlobalLocation() + new Vector3(0, 0, -0.45f));

        var bar = this.FindChildByType<TextureProgress>();
        var currentTick = GetTree().Root.FindChildByType<Default>().CurrentTick;

        if (currentTick >= sta.StationState.CooldownWillBeUpAt){
            bar.Visible = false;
        } else {
            bar.Visible = true;
            bar.Value = (sta.StationState.CooldownWillBeUpAt - currentTick) * 100 / sta.Cooldown;
        }
    }
}
