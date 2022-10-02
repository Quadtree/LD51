using System;
using System.Collections.Generic;
using Godot;

public class Default : Spatial, CartAction.IMutableGameState
{
    public int CurrentTick { get; set; }

    float Charge;

    public bool Paused = false;

    Station StationOnCursor;

    public float Money = 500;

    public List<Recipe> RecipeQueue = new List<Recipe>();

    public Recipe GetNextRecipe()
    {
        var ret = RecipeQueue[0];
        RecipeQueue.RemoveAt(0);
        return ret;
    }

    public override void _Ready()
    {
        RecipeQueue.Add(Recipes.SimpleSalad);
        RecipeQueue.Add(Recipes.SimpleSalad);
        RecipeQueue.Add(Recipes.ChoppedSalad);
        RecipeQueue.Add(Recipes.SimpleSalad);
        RecipeQueue.Add(Recipes.ChoppedSalad);
        RecipeQueue.Add(Recipes.ChoppedSalad);
        RecipeQueue.Add(Recipes.ChoppedSalad);
        RecipeQueue.Add(Recipes.MixedSalad);
        RecipeQueue.Add(Recipes.MixedSalad);
        RecipeQueue.Add(Recipes.MixedSalad);
        RecipeQueue.Add(Recipes.Soup);
        RecipeQueue.Add(Recipes.Soup);
        RecipeQueue.Add(Recipes.MixedSalad);
        RecipeQueue.Add(Recipes.MixedSalad);
        RecipeQueue.Add(Recipes.MixedSalad);
        RecipeQueue.Add(Recipes.Soup);
        RecipeQueue.Add(Recipes.Soup);
        RecipeQueue.Add(Recipes.SimpleSalad);
        RecipeQueue.Add(Recipes.MixedSalad);
        RecipeQueue.Add(Recipes.MixedSalad);
        RecipeQueue.Add(Recipes.MixedSalad);
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

        Money += delta * 10;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event.IsActionPressed("build_station_0")) LoadStationOnCursor("res://actors/stations/LettuceStation.tscn");
        if (@event.IsActionPressed("build_station_1")) LoadStationOnCursor("res://actors/stations/ChopStation.tscn");



        if (@event.IsActionPressed("cancel_plan"))
        {
            StationOnCursor = null;
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed("place_plan") && StationOnCursor != null)
        {
            GD.Print("place_plan");
            if (StationOnCursor.Cost <= Money)
            {
                Money -= StationOnCursor.Cost;
                StationOnCursor.Built = true;
                foreach (var it in StationOnCursor.FindChildrenByType<MeshInstance>())
                {
                    it.MaterialOverride = null;
                }
                // TODO: Money!
                StationOnCursor = null;
            }
            else
            {
                GD.Print("Insufficient funds");
                // TODO: Sound effect?
            }
        }
    }

    void LoadStationOnCursor(string path)
    {
        if (StationOnCursor != null) StationOnCursor.QueueFree();

        StationOnCursor = GD.Load<PackedScene>(path).Instance<Station>();
        StationOnCursor.Built = false;
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
