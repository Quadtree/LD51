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

        for (var i = 0; i < 10; ++i)
        {
            var recp = Util.Choice(Recipes.AllRecipes);
            while (Util.RandChanceMil(350))
            {
                Default.NextRecipeQueue.Add(recp);
            }
        }

        GetTree().ChangeScene("res://maps/Default.tscn");
    }
}
