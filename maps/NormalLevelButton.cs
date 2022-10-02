using System;
using Godot;

public class NormalLevelButton : Button
{
    public override void _Ready()
    {
        this.Connect("pressed", this, nameof(OnPressed));
    }

    void OnPressed()
    {
        Default.NextRecipeQueue = new System.Collections.Generic.List<Recipe>();

        Default.NextRecipeQueue.Add(Recipes.SimpleSalad);
        Default.NextRecipeQueue.Add(Recipes.SimpleSalad);
        Default.NextRecipeQueue.Add(Recipes.ChoppedSalad);
        Default.NextRecipeQueue.Add(Recipes.SimpleSalad);
        Default.NextRecipeQueue.Add(Recipes.ChoppedSalad);
        Default.NextRecipeQueue.Add(Recipes.ChoppedSalad);
        Default.NextRecipeQueue.Add(Recipes.ChoppedSalad);
        Default.NextRecipeQueue.Add(Recipes.MixedSalad);
        Default.NextRecipeQueue.Add(Recipes.MixedSalad);
        Default.NextRecipeQueue.Add(Recipes.MixedSalad);
        Default.NextRecipeQueue.Add(Recipes.Soup);
        Default.NextRecipeQueue.Add(Recipes.Soup);
        Default.NextRecipeQueue.Add(Recipes.MixedSalad);
        Default.NextRecipeQueue.Add(Recipes.MixedSalad);
        Default.NextRecipeQueue.Add(Recipes.MixedSalad);
        Default.NextRecipeQueue.Add(Recipes.Soup);
        Default.NextRecipeQueue.Add(Recipes.Soup);
        Default.NextRecipeQueue.Add(Recipes.SimpleSalad);
        Default.NextRecipeQueue.Add(Recipes.MixedSalad);
        Default.NextRecipeQueue.Add(Recipes.MixedSalad);
        Default.NextRecipeQueue.Add(Recipes.MixedSalad);

        GetTree().ChangeScene("res://maps/Default.tscn");
    }
}