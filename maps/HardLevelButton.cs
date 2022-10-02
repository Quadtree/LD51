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
        Default.NextRecipeQueue.Add(Recipes.Soup);
        Default.NextRecipeQueue.Add(Recipes.Soup);
        Default.NextRecipeQueue.Add(Recipes.Sandwich);

        GetTree().ChangeScene("res://maps/Default.tscn");
    }
}
