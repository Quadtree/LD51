using System;
using Godot;

public class Default : Spatial, CartAction.IMutableGameState
{
    public int CurrentTick { get; set; }

    float Charge;

    public bool Paused = false;

    Station StationOnCursor;

    public override void _Ready()
    {

    }

    public override void _Process(float delta)
    {
        if (!Paused) Charge += delta;

        if (Charge >= Cart.CART_MOVE_TIME)
        {
            Charge -= Cart.CART_MOVE_TIME;

            if (CurrentTick % 20 == 0)
            {
                GD.Print("Spawning!");
                this.AddChild(GD.Load<PackedScene>("res://actors/Cart.tscn").Instance<Cart>());
            }


            CurrentTick++;
            GD.Print($"CurrentTick={CurrentTick}");
        }

        if (StationOnCursor != null)
        {
            StationOnCursor.Reposition(Picking.PickPointAtCursor(this).Value);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event.IsActionPressed("build_station_0"))
        {
            LoadStationOnCursor("res://actors/stations/LettuceStation.tscn");
        }

        if (@event.IsActionPressed("place_plan") && StationOnCursor != null)
        {
            StationOnCursor.Built = true;
            // TODO: Money!
            StationOnCursor = null;
        }

        if (@event.IsActionPressed("cancel_plan"))
        {
            StationOnCursor = null;
        }
    }

    void LoadStationOnCursor(string path)
    {
        StationOnCursor = GD.Load<PackedScene>(path).Instance<Station>();
        GetTree().CurrentScene.AddChild(StationOnCursor);

        foreach (var it in StationOnCursor.FindChildrenByType<MeshInstance>())
        {
            var spat = (SpatialMaterial)(it.GetActiveMaterial(0)).Duplicate();
            spat.FlagsTransparent = true;
            spat.AlbedoColor = new Color(1, 1, 1, 0.5f);
            it.MaterialOverride = spat;
        }
    }

    public CartState GetCartState(int id)
    {
        return this.FindChildByPredicate<Cart>(it => it.ID == id).CurrentCartState;
    }

    public void SetCartState(int id, CartState state)
    {
        this.FindChildByPredicate<Cart>(it => it.ID == id).CurrentCartState = state;
    }

    public StationState GetStationState(int id)
    {
        return this.FindChildByPredicate<Station>(it => it.ID == id).StationState;
    }

    public void SetStationState(int id, StationState state)
    {
        this.FindChildByPredicate<Station>(it => it.ID == id).StationState = state;
    }
}
