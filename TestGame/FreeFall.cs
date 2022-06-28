using System.Numerics;
using SkiaGame;
using SkiaSharp;

namespace TestGame;

public class FreeFall : Engine
{
    private readonly GameObject _ball = new()
    {
        Position = new Vector2(400, 0),
        Primitive = Primitive.Circle,
        Size = new SKSize(10, 10)
    };

    private readonly GameObject _racket = new()
    {
        Position = new Vector2(400, 500),
        Primitive = Primitive.Rect,
        Size = new SKSize(100, 10),
        ReactToCollision = false
    };


    protected override void OnStart()
    {
        AddPhysics(_ball);
        AddPhysics(_racket);
        AddToDrawQueue(_ball);
        AddToDrawQueue(_racket);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
        if (_ball.Position.Y > e.Info.Height)
        {
            _ball.Position = new Vector2(e.Info.Width / 2.0f, 0);
        }
    }

    protected override void OnPhysicsUpdate(float timeStep)
    {
    }
}