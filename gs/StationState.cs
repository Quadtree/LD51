using Godot;

public struct StationState
{
    public int ID;

    public Recipe.Ing Ing;

    public int Duration;
    public int Cooldown;

    public void UseOn(GameState gs, int CartID)
    {
        var ncs = gs.CartStates[CartID].Clone();

        ncs.Ings.Add(Ing);

        gs.CartStates[CartID] = ncs;
    }
}