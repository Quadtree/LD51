using Godot;

public struct StationState
{
    public int ID;

    public Recipe.Ing Ing;

    public int Duration;
    public int Cooldown;

    public int CooldownWillBeUpAt;

    public void UseOn(CartAction.IMutableGameState gs, int CartID, bool printDebugData = false)
    {
        AT.True(Ing != Recipe.Ing.None);
        AT.True(gs.CurrentTick >= CooldownWillBeUpAt);
        var ncs = gs.GetCartState(CartID).Clone();

        ncs.Ings.Add(Ing);
        ncs.CanTakeNextActionAt = gs.CurrentTick + Duration;

        gs.SetCartState(CartID, ncs);


        var nss = gs.GetStationState(ID).Clone();
        nss.CooldownWillBeUpAt = gs.CurrentTick + Cooldown;
        gs.SetStationState(ID, nss);

        if (printDebugData) GD.Print($"Station {ID} has been used at tick {gs.CurrentTick}. CooldownWillBeUpAt={CooldownWillBeUpAt}");
    }

    public StationState Clone()
    {
        return this;
    }
}