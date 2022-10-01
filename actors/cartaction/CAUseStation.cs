public class CAUseStation : CartAction
{
    int StationID;

    public override void Execute(GameState state)
    {
        state.StationStates[StationID].UseOn(state, CartID);
    }
}