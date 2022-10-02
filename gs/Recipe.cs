using System.Collections.Generic;
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
    public string Name;

    public Recipe(string name, params Ing[] ings)
    {
        this.Ings = ings;
        this.Name = name;
    }
}

public class Recipes
{
    public static Recipe SimpleSalad = new Recipe("Simple Salad", Recipe.Ing.Lettuce);
    public static Recipe ChoppedSalad = new Recipe("Chopped Salad", Recipe.Ing.Lettuce, Recipe.Ing.Chop);
    public static Recipe MixedSalad = new Recipe("Mixed Salad", Recipe.Ing.Lettuce, Recipe.Ing.Tomato, Recipe.Ing.Chop);
    public static Recipe Soup = new Recipe("Soup", Recipe.Ing.Water, Recipe.Ing.Tomato, Recipe.Ing.Cook);
    public static Recipe Sandwich = new Recipe("Sandwich", Recipe.Ing.Bread, Recipe.Ing.Tomato, Recipe.Ing.Protein);
    public static Recipe ToastedSandwich = new Recipe("Toasted Sandwich", Recipe.Ing.Bread, Recipe.Ing.Tomato, Recipe.Ing.Protein, Recipe.Ing.Cook);
    public static Recipe Water = new Recipe("Water", Recipe.Ing.Water);
    public static Recipe Tea = new Recipe("Tea", Recipe.Ing.Water, Recipe.Ing.Cook);
    public static Recipe HugeSalad = new Recipe("HugeSalad", Recipe.Ing.Lettuce, Recipe.Ing.Lettuce, Recipe.Ing.Lettuce);

    public static Recipe[] AllRecipes = new Recipe[]{
SimpleSalad,ChoppedSalad,MixedSalad,Soup,Sandwich,ToastedSandwich,Water,Tea,HugeSalad
    };
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
        Data[Recipe.Ing.Lettuce] = new IngModel { Path = "res://models/lettuce.tscn", Height = 0.2f };
        Data[Recipe.Ing.Protein] = new IngModel { Path = "res://models/protein.tscn", Height = 0.2f };
        Data[Recipe.Ing.Bread] = new IngModel { Path = "res://models/bread.tscn", Height = 0.3f };
        Data[Recipe.Ing.Tomato] = new IngModel { Path = "res://models/tomato.tscn", Height = 0.4f };
        Data[Recipe.Ing.Water] = new IngModel { Path = "res://models/water.tscn", Height = 0.8f };

        Data[Recipe.Ing.Chop] = new IngModel { Path = "res://models/chop.tscn", Height = 0.2f };
        Data[Recipe.Ing.Cook] = new IngModel { Path = "res://models/cook.tscn", Height = 0.4f };
    }
}