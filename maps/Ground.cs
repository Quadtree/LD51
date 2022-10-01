using System;
using Godot;

public class Ground : MultiMeshInstance
{
    public const int WIDTH = 12;
    public const int HEIGHT = 8;

    public override void _Ready()
    {
        var it = this;
        var mm = it.Multimesh;
        mm.TransformFormat = MultiMesh.TransformFormatEnum.Transform3d;

        mm.InstanceCount = WIDTH * HEIGHT;

        for (var x = 0; x < WIDTH; ++x)
        {
            for (var y = 0; y < HEIGHT; ++y)
            {
                mm.SetInstanceTransform(x * HEIGHT + y, new Transform(Quat.Identity, new Vector3(x, 0, y)));
            }
        }
    }
}
