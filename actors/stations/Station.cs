using System.Collections.Generic;
using System.Linq;
using Godot;

public class Station : Spatial
{
    public int ID;

    [Export]
    public Recipe.Ing IngredientDelivered;

    [Export]
    public int Cooldown;

    [Export]
    public int Duration;

    [Export]
    public float Cost;

    public bool Built = true;

    public StationState StationState;

    public IntVec2 IntPos => new IntVec2(
            Mathf.RoundToInt(this.GetGlobalLocation().x),
            Mathf.RoundToInt(this.GetGlobalLocation().z)
        );

    public IEnumerable<IntVec2> GetBlocked()
    {
        yield return IntPos;
    }

    public void Reposition(Vector3 v3)
    {
        this.SetGlobalLocation(new Vector3(
            Mathf.RoundToInt(v3.x),
            0,
            Mathf.RoundToInt(v3.z)
        ));
    }

    public override void _Ready()
    {
        ID = GetTree().Root.FindChildrenByType<Station>().Select(it => it.ID).Max() + 1;

        StationState.Cooldown = Cooldown;
        StationState.Duration = Duration;
        StationState.Ing = IngredientDelivered;
        StationState.ID = ID;

        var rep = GD.Load<PackedScene>(IngModels.Data[IngredientDelivered].Path).Instance<Spatial>();
        rep.Translation = new Vector3(0, 0, 0);
        rep.Scale = Vector3.One * 0.1f;
        this.FindChildByName<Spatial>("Top").AddChild(rep);
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