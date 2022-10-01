public class CAMove : CartAction
{
    public IntVec2 Dest;

    public override void Execute(CartsState state)
    {
        state.Pos = Dest;
    }
}