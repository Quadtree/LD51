using System.Collections.Generic;

public class CartState
{
    public int ID;
    public IntVec2 Pos;
    public int Facing;

    public List<Recipe.Ing> Ings = new List<Recipe.Ing>();
}