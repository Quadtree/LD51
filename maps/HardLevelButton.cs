using System;
using Godot;

public class HardLevelButton : Button
{
    public override void _Ready()
    {
        this.Connect("pressed", this, nameof(OnPressed));
    }

    void OnPressed()
    {
        Default.NextRecipeQueue = new System.Collections.Generic.List<Recipe>();

        Default.NextRecipeQueue.Add(Recipes.Soup);
        Default.NextRecipeQueue.Add(Recipes.Tea);
        Default.NextRecipeQueue.Add(Recipes.Soup);
        Default.NextRecipeQueue.Add(Recipes.Tea);
        Default.NextRecipeQueue.Add(Recipes.ChoppedSalad);
        Default.NextRecipeQueue.Add(Recipes.ChoppedSalad);
        Default.NextRecipeQueue.Add(Recipes.ChoppedSalad);
        Default.NextRecipeQueue.Add(Recipes.MeatAndBread);
        Default.NextRecipeQueue.Add(Recipes.Sandwich);
        Default.NextRecipeQueue.Add(Recipes.MeatAndBread);
        Default.NextRecipeQueue.Add(Recipes.Sandwich);
        Default.NextRecipeQueue.Add(Recipes.HeartySoup);
        Default.NextRecipeQueue.Add(Recipes.HugeSalad);
        Default.NextRecipeQueue.Add(Recipes.HugeSalad);
        Default.NextRecipeQueue.Add(Recipes.MixedSalad);
        Default.NextRecipeQueue.Add(Recipes.HeartySoup);
        Default.NextRecipeQueue.Add(Recipes.ToastedSandwich);
        Default.NextRecipeQueue.Add(Recipes.MegaSandwich);
        Default.NextRecipeQueue.Add(Recipes.ToastedSandwich);
        Default.NextRecipeQueue.Add(Recipes.MegaSandwich);
        Default.NextRecipeQueue.Add(Recipes.MegaSandwich);

        Default.BronzeScore = Default.NextRecipeQueue.Count * 20;
        Default.SilverScore = Default.NextRecipeQueue.Count * 30;
        Default.GoldScore = Default.NextRecipeQueue.Count * 40;

        GetTree().ChangeScene("res://maps/Default.tscn");
    }
}
