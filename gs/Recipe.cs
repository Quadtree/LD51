using System.Text.RegularExpressions;
using Godot.Collections;

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
    public static Recipe ChoppedSalad = new Recipe(Recipe.Ing.Lettuce, Recipe.Ing.Chop);
    public static Recipe MixedSalad = new Recipe(Recipe.Ing.Lettuce, Recipe.Ing.Tomato, Recipe.Ing.Chop);
    public static Recipe Soup = new Recipe(Recipe.Ing.Water, Recipe.Ing.Tomato, Recipe.Ing.Cook);
    public static Recipe Sandwich = new Recipe(Recipe.Ing.Bread, Recipe.Ing.Tomato, Recipe.Ing.Protein);
    public static Recipe ToastedSandwich = new Recipe(Recipe.Ing.Bread, Recipe.Ing.Tomato, Recipe.Ing.Protein, Recipe.Ing.Cook);
}

public struct IngModel
{
    public string Path;
    public float Height;
}

public static class IngModels
{
    public static Dictionary<Recipe.Ing, IngModel> Data;

    static IngModels()
    {
        Data = new Dictionary<Recipe.Ing, IngModel>();
        Data[Recipe.Ing.Lettuce] = new IngModel { Path = "res://models/lettuce.glb", Height = 0.2f };
        Data[Recipe.Ing.Protein] = new IngModel { Path = "res://models/protein.glb", Height = 0.2f };
        Data[Recipe.Ing.Bread] = new IngModel { Path = "res://models/bread.glb", Height = 0.3f };
        Data[Recipe.Ing.Tomato] = new IngModel { Path = "res://models/tomato.glb", Height = 0.4f };
        Data[Recipe.Ing.Water] = new IngModel { Path = "res://models/water.glb", Height = 0.8f };
    }
}