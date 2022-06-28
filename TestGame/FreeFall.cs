using System.Numerics;
using SkiaGame;
using SkiaGame.Physics;
using SkiaSharp;

namespace TestGame;

public class FreeFall : Engine
{
    private Body _body = new Body();

    protected override void OnStart()
    {
        _body.Position = new Vector2(100, 0);
        AddBody(_body);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);
        var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Center,
            TextSize = 24
        };

        if (_body.Position.Y > e.Info.Height)
        {
            _body.Position = new Vector2(e.Info.Width / 2.0f, 0);
        }

        canvas.DrawCircle(_body.Position.X, _body.Position.Y, 5, paint);
    }

    protected override void OnPhysicsUpdate(float timeStep)
    {
    }
}