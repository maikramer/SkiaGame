using System.Numerics;
using SkiaGame.Events;
using SkiaSharp;

namespace SkiaGame.Input;

public class TouchKeys
{
    private SKRect _left;
    private SKRect _right;
    private SKRect _up;
    private SKRect _down;
    public float ButtonSize { get; set; } = 30;
    public float ControlSize { get; set; } = 100;
    public float TouchTolerance { get; set; } = 10;

    public SKPaint Paint { get; set; } = new()
    {
        Color = new SKColor(0, 0, 0, 30),
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        TextAlign = SKTextAlign.Center,
        TextSize = 24
    };

    public void DrawFromCenter(SKCanvas canvas, Vector2 center)
    {
        Draw(canvas, new Vector2(center.X - ControlSize / 2, center.Y - ControlSize / 2));
    }

    public void Draw(SKCanvas canvas, Vector2 position)
    {
        var firstOffset = ControlSize / 6;
        var secondOffset = ControlSize / 2;
        var thirdOffset = 5 * ControlSize / 6;
        var radius = ButtonSize / 2;
        var centerUpDown = position.X + secondOffset;
        var centerLeftRight = position.Y + secondOffset;
        //Cima
        canvas.DrawCircle(centerUpDown, position.Y + firstOffset, radius, Paint);
        _up = SKRect.Create(new SKPoint(centerUpDown - radius, position.Y + firstOffset - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Baixo
        canvas.DrawCircle(centerUpDown, position.Y + thirdOffset, radius, Paint);
        _down = SKRect.Create(new SKPoint(centerUpDown - radius, position.Y + thirdOffset - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Esquerda
        canvas.DrawCircle(position.X + firstOffset, centerLeftRight, radius, Paint);
        _left = SKRect.Create(new SKPoint(position.X + firstOffset - radius, centerLeftRight - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Direita
        canvas.DrawCircle(position.X + thirdOffset, centerLeftRight, radius, Paint);
        _right = SKRect.Create(new SKPoint(position.X + thirdOffset - radius, centerLeftRight - radius),
            new SKSize(ButtonSize, ButtonSize));
    }

    public TouchKey VerifyTouchCollision(Vector2 touchPoint)
    {
        var touchRect = SKRect.Create(new SKPoint(touchPoint.X - TouchTolerance / 2, touchPoint.Y - TouchTolerance / 2),
            new SKSize(TouchTolerance, TouchTolerance));
        var key = TouchKey.None;
        if (touchRect.IntersectsWithInclusive(_left))
        {
            key = TouchKey.Left;
        }
        else if (touchRect.IntersectsWith(_right))
        {
            key = TouchKey.Right;
        }

        if (touchRect.IntersectsWith(_up))
        {
            key = TouchKey.Up;
        }
        else if (touchRect.IntersectsWith(_down))
        {
            key = TouchKey.Down;
        }

        return key;
    }
}