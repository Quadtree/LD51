using Godot;

public struct StationState
{
    public int ID;

    public Recipe.Ing Ing;

    public int Duration;
    public int Cooldown;

    public void UseOn(CartAction.IMutableGameState gs, int CartID)
    {
        var ncs = gs.GetCartState(CartID).Clone();

        ncs.Ings.Add(Ing);

        gs.SetCartState(CartID, ncs);
    }
}