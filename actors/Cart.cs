using System.Linq;
using Godot;

public class Cart : Spatial
{
    // carts follow a series of steps
    // carts take 1 move per 0.5 seconds

    const float CART_MOVE_TIME = 0.5f;

    public int StartTick;

    public int ID;

    public Recipe Recipe;

    public CartState CurrentCartState;

    public override void _Ready()
    {
        ID = GetTree().Root.FindChildrenByType<Cart>().Select(it => it.ID).Max() + 1;

        StartTick = GetTree().Root.FindChildByType<Default>().CurrentTick;

        CurrentCartState = new CartState
        {
            Facing = 0,
            Ings = new System.Collections.Generic.List<Recipe.Ing>(),
            Pos = new IntVec2(-10, -10)
        };
    }
}