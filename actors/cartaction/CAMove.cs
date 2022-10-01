public class CAMove : CartAction
{
    public IntVec2 Dest;
    public int Facing;

    public override void Execute(GameState state)
    {
        var newCartState = state.CartStates[CartID];
        newCartState.Pos = Dest;
        newCartState.Facing = Facing;
        state.CartStates[CartID] = newCartState;
    }
}