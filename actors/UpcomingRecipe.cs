using System;
using Godot;

public class UpcomingRecipe : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public Recipe Recipe;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.FindChildByType<Label>().Text = $"{Recipe.Name}";
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //
    //  }
}
