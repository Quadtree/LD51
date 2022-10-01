using System.Collections.Generic;
using System.Linq;

public class GameState
{
    public Dictionary<int, CartState> CartStates;

    public Dictionary<int, StationState> StationStates;

    public GameState Clone()
    {
        return new GameState
        {
            CartStates = CartStates.ToDictionary(it => it.Key, it => it.Value),
            StationStates = StationStates.ToDictionary(it => it.Key, it => it.Value),
        };
    }
}