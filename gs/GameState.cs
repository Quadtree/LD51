using System.Collections.Generic;
using System.Linq;

public class GameState : CartAction.IMutableGameState
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
            CurrentTick = CurrentTick,
        };
    }

    public CartState GetCartState(int id) { return CartStates[id]; }
    public void SetCartState(int id, CartState state) { CartStates[id] = state; }
    public StationState GetStationState(int id) { return StationStates[id]; }
    public void SetStationState(int id, StationState state) { StationStates[id] = state; }
}