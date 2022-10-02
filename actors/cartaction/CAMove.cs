public class CAMove : CartAction
{
    public IntVec2 Dest;
    public int Facing;

    public override void Execute(IMutableGameState state, bool printDebugData)
    {
        var newCartState = state.GetCartState(CartID);
        newCartState.Pos = Dest;
        if (newCartState.Facing != Facing)
        {
            newCartState.Facing = Facing;
            newCartState.TurnsLeft--;
            //AT.True(newCartState.TurnsLeft >= 0);
        }

        state.SetCartState(CartID, newCartState);
    }

    public override string ToString()
    {
        return $"CAMove({Dest}, {Facing})";
    }
}