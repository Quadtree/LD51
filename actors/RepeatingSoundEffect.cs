using System;
using Godot;

public class RepeatingSoundEffect : Node
{
    public float TimeLeft = 0;
    public string Path;

    public override void _Ready()
    {
        this.FindChildByType<AudioStreamPlayer>().Stream = GD.Load<AudioStream>(Path);
        this.FindChildByType<AudioStreamPlayer>().Play();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        TimeLeft -= delta;
        if (TimeLeft <= 0) QueueFree();

        if (!this.FindChildByType<AudioStreamPlayer>().Playing) this.FindChildByType<AudioStreamPlayer>().Play();
    }

    public static void CreateRepeatingAudio(Node ctx, string path, float dur)
    {
        var n = GD.Load<PackedScene>("res://actors/RepeatingSoundEffect.tscn").Instance<RepeatingSoundEffect>();
        n.Path = path;
        n.TimeLeft = dur;
        ctx.GetTree().CurrentScene.AddChild(n);
    }
}
