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
    public static float Score = 0;

    public static float BronzeScore = 1;
    public static float SilverScore = 2;
    public static float GoldScore = 3;

    public List<Recipe> RecipeQueue = new List<Recipe>();

    public static List<Recipe> NextRecipeQueue;

    public List<UpcomingRecipe> UpcomingRecipes = new List<UpcomingRecipe>();

    public Recipe GetNextRecipe()
    {
        var ret = RecipeQueue[0];
        RecipeQueue.RemoveAt(0);
        return ret;
    }

    public override void _Ready()
    {
        Default.Score = 0;

        if (NextRecipeQueue == null)
        {
            //RecipeQueue.Add(Recipes.HugeSalad);
            RecipeQueue.Add(Recipes.Soup);
            RecipeQueue.Add(Recipes.Soup);
        }
        else
        {
            RecipeQueue = NextRecipeQueue;
            NextRecipeQueue = null;
        }

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

            CurrentTick++;
            GD.Print($"CurrentTick={CurrentTick}");

            if (CurrentTick % 20 == 0 && CurrentTick >= 20)
            {
                if (UpcomingRecipes.Count > 0)
                {
                    GD.Print("Spawning!");
                    UpcomingRecipes[0].QueueFree();
                    UpcomingRecipes.RemoveAt(0);
                    var cart = GD.Load<PackedScene>("res://actors/Cart.tscn").Instance<Cart>();
                    this.AddChild(cart);
                    cart.SetGlobalLocation(new Vector3(-10, 0, -10));
                }
            }
        }

        var timeLeft = !Paused ? 1 - ((CurrentTick % 20 + (Charge / Cart.CART_MOVE_TIME)) / 20f) : 0;

        foreach (var it in UpcomingRecipes)
        {
            it.SetGlobalLocation(new Vector3(-3, 0, timeLeft * 1.5f));
            timeLeft += 1;
        }

        if (StationOnCursor != null)
        {
            StationOnCursor.Reposition(Picking.PickPointAtCursor(this).Value);
        }

        if (!Paused) Money += delta * 10;

        if (UpcomingRecipes.Count == 0 && !GetTree().Root.FindChildrenByType<Cart>().Any())
        {
            GetTree().ChangeScene("res://maps/ScoreScreen.tscn");
        }
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
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed("place_plan") && StationOnCursor != null)
        {
            GD.Print("place_plan");
            if (StationOnCursor.Cost <= Money &&
            !StationOnCursor.GetBlocked().ToArray().Intersect(GetTree().Root.FindChildrenByType<Station>().Where(it => it.Built).SelectMany(it => it.GetBlocked())).Any() &&
            StationOnCursor.IntPos != Cart.ExitPoint &&
            StationOnCursor.IntPos != Cart.StartPoint)
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

        if (@event.IsActionPressed("cancel_plan") && StationOnCursor != null)
        {
            StationOnCursor.QueueFree();
            StationOnCursor = null;
        }
    }

    public void LoadStationOnCursor(string path)
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
