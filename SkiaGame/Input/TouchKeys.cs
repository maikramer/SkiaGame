using System.Numerics;
using SkiaSharp;

namespace SkiaGame.Input;

public class TouchKeys
{
    public float ButtonSize { get; set; } = 30;
    public float ControlSize { get; set; } = 100;
    public float TouchTolerance { get; set; } = 10;

    public SKPaint Paint { get; set; } = new()
    {
        Color = new SKColor(0, 30, 50, 30),
        IsAntialias = true,
        Style = SKPaintStyle.Fill
    };

    public SKPaint PaintArrows { get; set; } = new()
    {
        Color = new SKColor(0, 0, 0, 80),
        IsAntialias = true,
        Style = SKPaintStyle.Fill
    };

    private TouchKey _down;
    private TouchKey _left;
    private TouchKey _right;
    private TouchKey _up;

    private readonly SKPoint[] _setaPoints =
    {
        new(0, 0), new(-0.25f, 0.25f), new(0, 0.5f), new(0, 0.375f),
        new(0.25f, 0.375f), new(0.25f, 0.125f), new(0, 0.125f), new(0, 0)
    };

    public TouchKeys()
    {
        var baseArrow = new SKPath();
        baseArrow.AddPoly(_setaPoints);
        //Translata para que fique localizada no canto superior esquerdo
        var translate = SKMatrix.CreateTranslation(0.25f, 0);
        baseArrow.Transform(translate);
        var scaleMatrix = SKMatrix.CreateScale(ButtonSize, ButtonSize);
        baseArrow.Transform(scaleMatrix);
        _left = new TouchKey(baseArrow);
        var up = new SKPath(baseArrow);
        up.Transform(SKMatrix.CreateRotation((float)(Math.PI / 2),
            up.Bounds.MidX, up.Bounds.MidY));
        _up = new TouchKey(up);
        var down = new SKPath(baseArrow);
        down.Transform(SKMatrix.CreateRotation((float)(-Math.PI / 2),
            down.Bounds.MidX, down.Bounds.MidY));
        _down = new TouchKey(down);
        var right = new SKPath(baseArrow);
        right.Transform(SKMatrix.CreateRotation((float)(Math.PI),
            right.Bounds.MidX, right.Bounds.MidY));
        _right = new TouchKey(right);
    }

    public void DrawFromCenter(SKCanvas canvas, Vector2 center)
    {
        Draw(canvas,
            new Vector2(center.X - ControlSize / 2,
                center.Y - ControlSize / 2));
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
        //Desenha o circulo
        canvas.DrawCircle(centerUpDown, position.Y + firstOffset, radius,
            Paint);
        _up.Bounds = SKRect.Create(
            new SKPoint(centerUpDown - radius,
                position.Y + firstOffset - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(_up.Arrow, PaintArrows);
        //Baixo
        //Desenha o circulo
        canvas.DrawCircle(centerUpDown, position.Y + thirdOffset, radius,
            Paint);
        _down.Bounds = SKRect.Create(
            new SKPoint(centerUpDown - radius,
                position.Y + thirdOffset - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(_down.Arrow, PaintArrows);
        //Esquerda
        //Desenha o circulo
        canvas.DrawCircle(position.X + firstOffset, centerLeftRight, radius,
            Paint);
        _left.Bounds = SKRect.Create(
            new SKPoint(position.X + firstOffset - radius,
                centerLeftRight - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(_left.Arrow, PaintArrows);
        //Direita
        //Desenha o circulo
        canvas.DrawCircle(position.X + thirdOffset, centerLeftRight, radius,
            Paint);
        _right.Bounds = SKRect.Create(
            new SKPoint(position.X + thirdOffset - radius,
                centerLeftRight - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(_right.Arrow, PaintArrows);
    }

    public Events.EventTouchKey VerifyTouchCollision(Vector2 touchPoint)
    {
        var touchRect = SKRect.Create(
            new SKPoint(touchPoint.X - TouchTolerance / 2,
                touchPoint.Y - TouchTolerance / 2),
            new SKSize(TouchTolerance, TouchTolerance));
        var key = Events.EventTouchKey.None;
        if (touchRect.IntersectsWithInclusive(_left.Bounds))
        {
            key = Events.EventTouchKey.Left;
        }
        else if (touchRect.IntersectsWith(_right.Bounds))
        {
            key = Events.EventTouchKey.Right;
        }

        if (touchRect.IntersectsWith(_up.Bounds))
        {
            key = Events.EventTouchKey.Up;
        }
        else if (touchRect.IntersectsWith(_down.Bounds))
        {
            key = Events.EventTouchKey.Down;
        }

        return key;
    }
}