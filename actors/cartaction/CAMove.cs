public class CAMove : CartAction
{
    public IntVec2 Dest;
    public int Facing;

    public override void Execute(GameState state)
    {
        var newCartState = state.CartStates[CartID];
        newCartState.Pos = Dest;
        if (newCartState.Facing != Facing)
        {
            newCartState.Facing = Facing;
            newCartState.TurnsLeft--;
            AT.True(newCartState.TurnsLeft >= 0);
        }
        state.CartStates[CartID] = newCartState;
    }
}