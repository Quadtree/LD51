public class CAUseStation : CartAction
{
    public int StationID;

    public override void Execute(IMutableGameState state, bool printDebugData)
    {
        state.GetStationState(StationID).UseOn(state, CartID, printDebugData);

        if (printDebugData){
            if (state.GetStationState(StationID).Ing == Recipe.Ing.Cook){
                
            }
        }
    }

    public override string ToString()
    {
        return $"CAUseStation({StationID})";
    }
}