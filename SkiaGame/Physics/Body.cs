using System.Numerics;
using SkiaSharp;

namespace SkiaGame.Physics;

public class Body
{
    public Vector2 Speed { get; set; }
    public Vector2 Position { get; set; }


    public void Update(float timeStep)
    {
        Position += Speed * timeStep;
    }
}