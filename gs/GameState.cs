using System.Collections.Generic;
using System.Linq;

public class GameState
{
    public int CurrentTick;

    public Dictionary<int, CartState> CartStates = new Dictionary<int, CartState>();

    public Dictionary<int, StationState> StationStates = new Dictionary<int, StationState>();

    public GameState Clone()
    {
        return new GameState
        {
            CartStates = CartStates.ToDictionary(it => it.Key, it => it.Value),
            StationStates = StationStates.ToDictionary(it => it.Key, it => it.Value),
        };
    }
}