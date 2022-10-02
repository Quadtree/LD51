using System.Collections.Generic;
using System.Linq;
using Godot;

public class Station : Spatial
{
    public int ID;

    [Export]
    public Recipe.Ing IngredientDelivered;

    [Export]
    int Cooldown;

    [Export]
    int Duration;

    public bool Built;

    public StationState StationState;

    public IntVec2 IntPos => new IntVec2(
            Mathf.RoundToInt(this.GetGlobalLocation().x),
            Mathf.RoundToInt(this.GetGlobalLocation().z)
        );

    public IEnumerable<IntVec2> GetBlocked()
    {
        yield return IntPos;
    }

    public override void _Ready()
    {
        ID = GetTree().Root.FindChildrenByType<Station>().Select(it => it.ID).Max() + 1;

        StationState.Cooldown = Cooldown;
        StationState.Duration = Duration;
        StationState.Ing = IngredientDelivered;
        StationState.ID = ID;
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