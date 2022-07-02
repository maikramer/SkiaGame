using System.Numerics;
using SkiaGame;
using SkiaGame.Events;
using SkiaSharp;

namespace TestGame;

public class RectBouncer : Engine
{
    private const float RectSize = 50;

    private const float Speed = 100;

    private readonly GameObject _gameObject = new()
    {
        Primitive = Primitive.Rect,
        Size = new SKSize(50, 50)
    };

    private bool _growX = true;
    private bool _growY = true;
    private SKPoint _lastPosition;


    protected override void OnStart()
    {
        AddToDrawQueue(_gameObject);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
        float nextX;
        if (_growX)
        {
            nextX = _lastPosition.X + Speed * timeStep;
            if (nextX > e.Info.Width - RectSize)
            {
                _growX = false;
                nextX = e.Info.Width - RectSize;
            }
        }
        else
        {
            nextX = _lastPosition.X - Speed * timeStep;
            if (nextX < 0)
            {
                _growX = true;
                nextX = 0;
            }
        }

        float nextY;
        if (_growY)
        {
            nextY = _lastPosition.Y + Speed * timeStep;
            if (nextY > e.Info.Height - RectSize)
            {
                _growY = false;
                nextY = e.Info.Height - RectSize;
            }
        }
        else
        {
            nextY = _lastPosition.Y - Speed * timeStep;
            if (nextY < 0)
            {
                _growY = true;
                nextY = 0;
            }
        }


        _gameObject.Position = new Vector2(nextX, nextY);
        _lastPosition.X = nextX;
        _lastPosition.Y = nextY;
    }

    protected override void BeforePhysicsUpdate(float timeStep)
    {
    }
}