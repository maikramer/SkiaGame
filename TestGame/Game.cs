using SkiaGame;
using SkiaSharp;

namespace TestGame;

public class Game : Engine
{
    private const float RectSize = 50;
    private const float Speed = 100;
    private SKPoint _lastPosition;
    private bool _growX = true;
    private bool _growY = true;

    // ReSharper disable once InconsistentNaming
    private static readonly Game? _instance = null;
    public static Game Instance => _instance ?? new Game();
    public Engine Engine => Instance;

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

        float nextX;
        if (_growX)
        {
            nextX = (_lastPosition.X + Speed * timeStep);
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


        var coord = new SKPoint(nextX, nextY);
        _lastPosition.X = nextX;
        _lastPosition.Y = nextY;
        var rect = new SKRect
        {
            Location = coord,
            Size = new SKSize(RectSize, RectSize)
        };
        canvas.DrawRect(rect, paint);
    }
}