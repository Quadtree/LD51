using Godot;

public class Cart : Spatial
{
    // carts follow a series of steps
    // carts take 1 move per 0.5 seconds

    const float CART_MOVE_TIME = 0.5f;

    int StartTick;

    Recipe Recipe;

    public override void _Ready()
    {
        StartTick = GetTree().Root.FindChildByType<Default>().CurrentTick;
    }
}