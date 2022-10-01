using System.Collections.Generic;
using Godot;

public class Station : Spatial
{
    int ID;

    [Export]
    Recipe.Ing IngredientDelivered;

    IEnumerable<IntVec2> GetBlocked()
    {
        yield return new IntVec2(
            Mathf.RoundToInt(this.GetGlobalLocation().x),
            Mathf.RoundToInt(this.GetGlobalLocation().z)
        );
    }
}