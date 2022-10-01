using System.Collections.Generic;
using Godot;

public class Station : Spatial
{
    public int ID;

    [Export]
    Recipe.Ing IngredientDelivered;

    public IEnumerable<IntVec2> GetBlocked()
    {
        yield return new IntVec2(
            Mathf.RoundToInt(this.GetGlobalLocation().x),
            Mathf.RoundToInt(this.GetGlobalLocation().z)
        );
    }

    public int UseOn(CartState cartState)
    {
        // this call should be stateless!
        cartState.Ings.Add(IngredientDelivered);
    }
}