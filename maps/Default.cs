using System;
using Godot;

public class Default : Spatial, CartAction.IMutableGameState
{
    public int CurrentTick { get; set; }

    float Charge;

    public bool Paused = false;

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
