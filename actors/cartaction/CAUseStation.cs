public class CAUseStation : CartAction
{
    public int StationID;

    public override void Execute(GameState state)
    {
        state.StationStates[StationID].UseOn(state, CartID);
    }
}