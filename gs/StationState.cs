public struct StationState
{
    public int ID;

    public Recipe.Ing Ing;

    public void UseOn(GameState gs, Station station, int CartID)
    {
        var ncs = gs.CartStates[CartID];
        station.UseOn(ncs);
        gs.CartStates[CartID] = ncs;
    }
}