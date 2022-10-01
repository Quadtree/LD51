using System.Collections.Generic;
using System.Linq;

public struct CartState
{
    public int ID;
    public IntVec2 Pos;
    public int Facing;
    public int TurnsLeft;

    public List<Recipe.Ing> Ings;

    public CartState Clone()
    {
        var ret = this;
        ret.Ings = Ings.ToList();
        return ret;
    }
}