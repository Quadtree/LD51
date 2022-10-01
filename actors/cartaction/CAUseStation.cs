public class CAUseStation : CartAction
{
    public int StationID;

    public override void Execute(IMutableGameState state)
    {
        state.GetStationState(StationID).UseOn(state, CartID);
    }

    public override string ToString()
    {
        return $"CAUseStation({StationID})";
    }
}