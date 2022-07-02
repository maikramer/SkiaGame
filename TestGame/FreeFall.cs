using System.Numerics;
using SkiaGame;
using SkiaGame.Events;
using SkiaSharp;

namespace TestGame;

public class FreeFall : Engine
{
    private const float CircleDiameter = 50;

    private readonly GameObject _ball = new()
    {
        Primitive = Primitive.Circle,
        Diameter = CircleDiameter
    };

    private readonly GameObject _racket = new()
    {
        Position = new Vector2(0, 500),
        Primitive = Primitive.Rect,
        Size = new SKSize(100, 10),
        Locked = true,
    };


    protected override void OnStart()
    {
        AddToEngine(_ball);
        AddToEngine(_racket);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
        _ball.CenterX();
        _racket.CenterX();
        if (_ball.Position.Y > e.Info.Height)
        {
            _ball.Position = new Vector2(e.Info.Width / 2.0f, 0);
        }
    }

    protected override void OnPhysicsUpdate(float timeStep)
    {
        
    }
}