using System.Numerics;
using SkiaGame;
using SkiaSharp;

namespace TestGame;

public class FreeFall : Engine
{
    private readonly GameObject _gameObject = new()
    {
        Position = new Vector2(100, 0),
        Primitive = Primitive.Circle,
        CircleRadius = 5
    };


    protected override void OnStart()
    {
        AddPhysics(_gameObject);
        AddToDrawQueue(_gameObject);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
        if (_gameObject.Position.Y > e.Info.Height)
        {
            _gameObject.Position = new Vector2(e.Info.Width / 2.0f, 0);
        }
    }

    protected override void OnPhysicsUpdate(float timeStep)
    {
    }
}