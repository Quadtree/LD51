using System.Collections.Generic;

public struct CartState
{
    public int ID;
    public IntVec2 Pos;
    public int Facing;

    public List<Recipe.Ing> Ings;
}