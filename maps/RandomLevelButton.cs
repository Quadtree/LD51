using System;
using Godot;

public class RandomLevelButton : Button
{
    public override void _Ready()
    {
        this.Connect("pressed", this, nameof(OnPressed));
    }

    void OnPressed()
    {
        Default.NextRecipeQueue = new System.Collections.Generic.List<Recipe>();

        for (var i = 0; i < 20; ++i)
        {
            Default.NextRecipeQueue.Add(Util.Choice(Recipes.AllRecipes));
        }

        GetTree().ChangeScene("res://maps/Default.tscn");
    }
}
