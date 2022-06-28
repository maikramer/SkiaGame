using System.Numerics;
using SkiaSharp;

namespace SkiaGame.Physics;

public class RigidBody
{
    public bool HasGravity { get; set; } = true;
    public Vector2 Speed { get; set; }
    public Vector2 Position { get; set; }

    public SKRect Bounds { get; set; } = new();


    public void Update(float timeStep)
    {
        Position += Speed * timeStep;
    }
}