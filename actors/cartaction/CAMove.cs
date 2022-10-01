public class CAMove : CartAction
{
    public IntVec2 Dest;

    public override void Execute(GameState state)
    {
        state.CartStates[CartID].Pos = Dest;
    }
}