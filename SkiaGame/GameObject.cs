using System.Numerics;
using SkiaGame.Physics;
using SkiaSharp;

namespace SkiaGame;

public class GameObject
{
    public RigidBody RigidBody { get; set; } = new RigidBody();
    public Primitive Primitive { get; set; } = Primitive.Rect;
    public float CircleRadius { get; set; } = 0;
    public SKSize RectSize { get; set; } = new(1, 1);
    public SKPoint RoundRectCornerRadius { get; set; } = new(1, 1);

    public Vector2 Position
    {
        get => RigidBody.Position;
        set => RigidBody.Position = value;
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
                canvas.DrawCircle(RigidBody.Position.X, RigidBody.Position.Y, CircleRadius, Paint);
                break;
            case Primitive.Rect:
                canvas.DrawRect(RigidBody.Position.X, RigidBody.Position.Y, RectSize.Width, RectSize.Height, Paint);
                break;
            case Primitive.RoundRect:
                canvas.DrawRoundRect(RigidBody.Position.X, RigidBody.Position.Y, RectSize.Width, RectSize.Height,
                    RoundRectCornerRadius.X, RoundRectCornerRadius.Y, Paint);
                break;
            case Primitive.Path:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}