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

        for (var i = 0; i < Recipe.Ings.Length; ++i)
        {
            var rep = GD.Load<PackedScene>(IngModels.Data[Recipe.Ings[i]].Path).Instance<Spatial>();
            rep.Translation = new Vector3(1 + i, 0, 0);
            rep.Scale = Vector3.One * 0.1f;
            AddChild(rep);
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var pos = GetTree().Root.FindChildByType<Camera>().UnprojectPosition(this.GetGlobalLocation());
        this.FindChildByType<Label>().RectPosition = pos;
    }
}
