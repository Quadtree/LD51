public abstract class CartAction
{
    public int CartID;

    public interface IMutableGameState
    {
        CartState GetCartState(int id);
        void SetCartState(int id, CartState state);
        StationState GetStationState(int id);
        void SetStationState(int id, StationState state);
    }

    public abstract void Execute(IMutableGameState state);
}