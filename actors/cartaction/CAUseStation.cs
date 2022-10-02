using Godot;

public class CAUseStation : CartAction
{
    public int StationID;

    public override void Execute(IMutableGameState state, bool printDebugData)
    {
        state.GetStationState(StationID).UseOn(state, CartID, printDebugData);
    }

    public override string ToString()
    {
        return $"CAUseStation({StationID})";
    }

    void CreateRepeatingAudio(string path, float dur)
    {
        var n = GD.Load<PackedScene>("res://actors/RepeatingSoundEffect.tscn").Instance<RepeatingSoundEffect>();
        n.Path = path;
        n.TimeLeft = dur;

    }
}