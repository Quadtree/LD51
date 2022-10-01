using System.Collections.Generic;
using Godot;

public class Station : Spatial
{
    public int ID;

    [Export]
    Recipe.Ing IngredientDelivered;

    [Export]
    int Cooldown;

    [Export]
    int Duration;

    public StationState StationState;

    public IEnumerable<IntVec2> GetBlocked()
    {
        yield return new IntVec2(
            Mathf.RoundToInt(this.GetGlobalLocation().x),
            Mathf.RoundToInt(this.GetGlobalLocation().z)
        );
    }

    public override void _Ready()
    {
        StationState.Cooldown = Cooldown;
        StationState.Duration = Duration;
        StationState.Ing = IngredientDelivered;
    }

    //public struct StationUse
    //{
    //    public int Cooldown;
    //    public int Duration;
    //}

    /*public StationUse UseOn(CartState cartState)
    {
        // this call should be stateless!

        cartState.Ings.Add(IngredientDelivered);

        // cooldown!
        return new StationUse
        {
            Cooldown = Cooldown,
            Duration = Duration,
        };
    }*/
}