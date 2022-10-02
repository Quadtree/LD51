using System.Text.RegularExpressions;

public class Recipe
{
    public enum Ing
    {
        None,
        Lettuce,
        Cook,
        Chop,
        Protein,
        Bread,
        Tomato,
        Water
    }

    public Ing[] Ings;

    public Recipe(params Ing[] ings)
    {
        this.Ings = ings;
    }

    private string _Name;

    public string Name
    {
        get
        {

            if (_Name == null)
            {
                var rx = new Regex("([a-z])([A-Z])");
                _Name = rx.Replace(GetType().Name, (it) => it.Groups[1].Value + " " + it.Groups[2].Value);
            }
            return _Name;
        }
    }
}

public class Recipes
{
    public static Recipe SimpleSalad = new Recipe(Recipe.Ing.Lettuce);
    public static Recipe ChoppedSalad = new Recipe(Recipe.Ing.Lettuce);
}