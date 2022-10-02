using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Default : Spatial, CartAction.IMutableGameState
{
    public int CurrentTick { get; set; }

    float Charge;

    public bool Paused = false;

    Station StationOnCursor;

    public float Money = 500;
    public float Score = 0;

    public List<Recipe> RecipeQueue = new List<Recipe>();

    public List<UpcomingRecipe> UpcomingRecipes = new List<UpcomingRecipe>();

    public Recipe GetNextRecipe()
    {
        var ret = RecipeQueue[0];
        RecipeQueue.RemoveAt(0);
        return ret;
    }

    public override void _Ready()
    {
        RecipeQueue.Add(Recipes.Soup);

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

        foreach (var it in RecipeQueue)
        {
            var rce = GD.Load<PackedScene>("res://actors/UpcomingRecipe.tscn").Instance<UpcomingRecipe>();
            rce.Recipe = it;
            UpcomingRecipes.Add(rce);
            AddChild(rce);
        }

        var n = 0;

        foreach (var it in Util.GetEnumValues<Recipe.Ing>().Skip(1))
        {
            var rce = GD.Load<PackedScene>("res://ui/BuyStationButton.tscn").Instance<BuyStationButton>();
            rce.Type = it;
            AddChild(rce);
            rce.SetGlobalLocation(new Vector3(12.5f, 0, n * 1f));
            ++n;
        }
    }

    public override void _Process(float delta)
    {
        if (!Paused) Charge += delta;

        if (Charge >= Cart.CART_MOVE_TIME)
        {
            Charge -= Cart.CART_MOVE_TIME;

            if (CurrentTick % 20 == 0 && CurrentTick >= 20)
            {
                GD.Print("Spawning!");
                UpcomingRecipes[0].QueueFree();
                UpcomingRecipes.RemoveAt(0);
                this.AddChild(GD.Load<PackedScene>("res://actors/Cart.tscn").Instance<Cart>());
            }

            CurrentTick++;
            GD.Print($"CurrentTick={CurrentTick}");
        }

        var timeLeft = 1 - ((CurrentTick % 20 + (Charge / Cart.CART_MOVE_TIME)) / 20f);

        foreach (var it in UpcomingRecipes)
        {
            it.SetGlobalLocation(new Vector3(-3, 0, timeLeft * 1.5f));
            timeLeft += 1;
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
        if (@event.IsActionPressed("build_station_2")) LoadStationOnCursor("res://actors/stations/BreadStation.tscn");
        if (@event.IsActionPressed("build_station_3")) LoadStationOnCursor("res://actors/stations/ProteinStation.tscn");
        if (@event.IsActionPressed("build_station_4")) LoadStationOnCursor("res://actors/stations/TomatoStation.tscn");
        if (@event.IsActionPressed("build_station_5")) LoadStationOnCursor("res://actors/stations/WaterStation.tscn");
        if (@event.IsActionPressed("build_station_6")) LoadStationOnCursor("res://actors/stations/CookStation.tscn");

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
