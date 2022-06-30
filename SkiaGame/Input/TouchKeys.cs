using System.Numerics;
using SkiaGame.Events;
using SkiaSharp;

namespace SkiaGame.Input;

public class TouchKeys
{
    private readonly SKPoint[] _setaPoints =
    {
        new(0, 0), new(-0.25f, 0.25f), new(0, 0.5f), new(0, 0.375f), new(0.25f, 0.375f),
        new(0.25f, 0.125f), new(0, 0.125f), new(0, 0)
    };

    private readonly List<Key> _keys = new();
    public Key Down;

    public Key Left;
    public Key Right;
    public Key Up;

    public TouchKeys()
    {
        var baseArrow = new SKPath();
        baseArrow.AddPoly(_setaPoints);
        //Translata para que fique localizada no canto superior esquerdo
        var translate = SKMatrix.CreateTranslation(0.25f, 0);
        baseArrow.Transform(translate);
        var scaleMatrix = SKMatrix.CreateScale(ButtonSize, ButtonSize);
        baseArrow.Transform(scaleMatrix);
        Left = new Key(TouchKeyEventCode.Left)
        {
            TouchKey = new TouchKey(baseArrow)
        };
        var up = new SKPath(baseArrow);
        up.Transform(SKMatrix.CreateRotation((float)(Math.PI / 2),
            up.Bounds.MidX, up.Bounds.MidY));
        Up = new Key(TouchKeyEventCode.Up)
        {
            TouchKey = new TouchKey(up)
        };
        var down = new SKPath(baseArrow);
        down.Transform(SKMatrix.CreateRotation((float)(-Math.PI / 2),
            down.Bounds.MidX, down.Bounds.MidY));
        Down = new Key(TouchKeyEventCode.Down)
        {
            TouchKey = new TouchKey(down)
        };
        var right = new SKPath(baseArrow);
        right.Transform(SKMatrix.CreateRotation((float)Math.PI,
            right.Bounds.MidX, right.Bounds.MidY));
        Right = new Key(TouchKeyEventCode.Right)
        {
            TouchKey = new TouchKey(right)
        };
        _keys.AddRange(new[]
        {
            Up, Down, Left, Right
        });
    }

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
        Up.TouchKey.Bounds = SKRect.Create(
            new SKPoint(centerUpDown - radius,
                position.Y + firstOffset - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(Up.TouchKey.Arrow, PaintArrows);
        //Baixo
        //Desenha o circulo
        canvas.DrawCircle(centerUpDown, position.Y + thirdOffset, radius,
            Paint);
        Down.TouchKey.Bounds = SKRect.Create(
            new SKPoint(centerUpDown - radius,
                position.Y + thirdOffset - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(Down.TouchKey.Arrow, PaintArrows);
        //Esquerda
        //Desenha o circulo
        canvas.DrawCircle(position.X + firstOffset, centerLeftRight, radius,
            Paint);
        Left.TouchKey.Bounds = SKRect.Create(
            new SKPoint(position.X + firstOffset - radius,
                centerLeftRight - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(Left.TouchKey.Arrow, PaintArrows);
        //Direita
        //Desenha o circulo
        canvas.DrawCircle(position.X + thirdOffset, centerLeftRight, radius,
            Paint);
        Right.TouchKey.Bounds = SKRect.Create(
            new SKPoint(position.X + thirdOffset - radius,
                centerLeftRight - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(Right.TouchKey.Arrow, PaintArrows);
    }

    public TouchKeyEventCode VerifyTouchCollision(Vector2 touchPoint,
        bool isPress)
    {
        var touchRect = SKRect.Create(
            new SKPoint(touchPoint.X - TouchTolerance / 2,
                touchPoint.Y - TouchTolerance / 2),
            new SKSize(TouchTolerance, TouchTolerance));
        var lastKeyPressed = TouchKeyEventCode.None;
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var key in _keys)
        {
            key.IsPressed = isPress &&
                            touchRect.IntersectsWithInclusive(key.TouchKey.Bounds);
            if (key.IsPressed)
            {
                lastKeyPressed = key.EventCode;
            }
        }

        return lastKeyPressed;
    }

    public class Key
    {
        public bool IsPressed;
        internal TouchKey TouchKey = new(new SKPath());

        public Key(TouchKeyEventCode eventCode)
        {
            EventCode = eventCode;
        }

        public TouchKeyEventCode EventCode { get; }
    }
}