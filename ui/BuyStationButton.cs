using System;
using Godot;

public class BuyStationButton : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public Recipe.Ing Type;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var rep = GD.Load<PackedScene>(IngModels.Data[Type].Path).Instance<Spatial>();
        rep.Translation = new Vector3(-0.4f, 0, 0);
        rep.Scale = Vector3.One * 0.1f;
        AddChild(rep);

        var ts = GD.Load<PackedScene>($"res://actors/stations/{Type}Station.tscn").Instance<Station>();

        this.FindChildByType<Button>().Text = Type.ToString() + $" (${ts.Cost})";
        this.FindChildByType<Button>().HintTooltip = $"Cost: ${ts.Cost}\nUse Time: {ts.Duration + 1}\nCooldown: {ts.Cooldown + 1}";
        this.FindChildByType<Button>().Connect("pressed", this, nameof(Pressed));
    }

    void Pressed()
    {
        GetTree().Root.FindChildByType<Default>().LoadStationOnCursor($"res://actors/stations/{Type}Station.tscn");
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var pos = GetTree().Root.FindChildByType<Camera>().UnprojectPosition(this.GetGlobalLocation());
        this.FindChildByType<Button>().RectPosition = pos;
    }
}
