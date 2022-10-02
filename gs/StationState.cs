using Godot;

public struct StationState
{
    public int ID;

    public Recipe.Ing Ing;

    public int Duration;
    public int Cooldown;

    public int CooldownWillBeUpAt;

    public void UseOn(CartAction.IMutableGameState gs, int CartID)
    {
        AT.True(Ing != Recipe.Ing.None);
        AT.True(gs.CurrentTick >= CooldownWillBeUpAt);
        var ncs = gs.GetCartState(CartID).Clone();

        ncs.Ings.Add(Ing);

        gs.SetCartState(CartID, ncs);


        var nss = gs.GetStationState(ID).Clone();
        CooldownWillBeUpAt = gs.CurrentTick + Cooldown;
        gs.SetStationState(ID, nss);
    }

    public StationState Clone()
    {
        return this;
    }
}