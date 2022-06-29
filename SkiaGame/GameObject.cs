using System.Numerics;
using SkiaGame.Physics;
using SkiaSharp;

namespace SkiaGame;

public class GameObject
{
    internal Engine? _engine;
    public RigidBody RigidBody { get; set; } = new();
    public Primitive Primitive { get; set; } = Primitive.Rect;

    public SKSize Size
    {
        get => RigidBody.Bounds.Size;
        set => RigidBody.SetSize(value);
    }

    public SKColor Color
    {
        get => Paint.Color;
        set => Paint.Color = value;
    }

    public float Diameter
    {
        get => Size.Width;
        set => Size = new SKSize(value, value);
    }

    public SKPoint RoundRectCornerRadius { get; set; } = new(1, 1);

    public Vector2 Position
    {
        get => RigidBody.Position;
        set => RigidBody.SetPosition(value);
    }

    public bool ReactToCollision
    {
        get => RigidBody.ReactToCollision;
        set => RigidBody.ReactToCollision = value;
    }

    public bool HasGravity
    {
        get => RigidBody.HasGravity;
        set => RigidBody.HasGravity = value;
    }

    public SKPaint Paint = new()
    {
        Color = SKColors.Black,
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        TextAlign = SKTextAlign.Center,
        TextSize = 24
    };

    public void Draw(SKCanvas canvas)
    {
        switch (Primitive)
        {
            case Primitive.Circle:
                canvas.DrawCircle(RigidBody.Bounds.MidX, RigidBody.Bounds.MidY, Size.Width / 2, Paint);
                break;
            case Primitive.Rect:
                canvas.DrawRect(RigidBody.Position.X, RigidBody.Position.Y,
                    Size.Width, Size.Height, Paint);
                break;
            case Primitive.RoundRect:
                canvas.DrawRoundRect(RigidBody.Position.X, RigidBody.Position.Y,
                    Size.Width, Size.Height,
                    RoundRectCornerRadius.X, RoundRectCornerRadius.Y, Paint);
                break;
            case Primitive.Path:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}