using System.Numerics;
using SkiaGame.Physics;
using SkiaSharp;

namespace SkiaGame;

public class GameObject
{
    public readonly SKPaint Paint = new()
    {
        Color = SKColors.Black,
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        TextAlign = SKTextAlign.Center,
        TextSize = 24
    };

    private Primitive _primitive = Primitive.Rect;

    internal Engine? Engine;

    public RigidBody RigidBody { get; set; } = new();

    public Primitive Primitive
    {
        get => _primitive;
        set
        {
            switch (value)
            {
                case Primitive.Circle:
                    RigidBody.ShapeType = RigidBody.Type.Circle;
                    break;
                case Primitive.Rect:
                    RigidBody.ShapeType = RigidBody.Type.Box;
                    break;
                case Primitive.RoundRect:
                    RigidBody.ShapeType = RigidBody.Type.Box;
                    break;
                case Primitive.Path:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            _primitive = value;
        }
    }

    public SKSize Size
    {
        get => RigidBody.Size;
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

    public bool Locked
    {
        get => RigidBody.Locked;
        set => RigidBody.Locked = value;
    }

    public bool HasGravity
    {
        get => RigidBody.HasGravity;
        set => RigidBody.HasGravity = value;
    }

    public void Draw(SKCanvas canvas)
    {
        lock (RigidBody)
        {
            switch (Primitive)
            {
                case Primitive.Circle:
                    canvas.DrawCircle(RigidBody.Center.X, RigidBody.Center.Y, Size.Width / 2,
                        Paint);
                    break;
                case Primitive.Rect:
                    canvas.DrawRect(RigidBody.Position.X, RigidBody.Position.Y, Size.Width,
                        Size.Height, Paint);
                    break;
                case Primitive.RoundRect:
                    canvas.DrawRoundRect(RigidBody.Position.X, RigidBody.Position.Y, Size.Width,
                        Size.Height, RoundRectCornerRadius.X, RoundRectCornerRadius.Y, Paint);
                    break;
                case Primitive.Path:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}