using System.Numerics;
using SkiaSharp;

namespace SkiaGame.Physics;

public class BoundingBox
{
    private SKRect _bounds;
    public RigidBody Down = new();
    public RigidBody Left = new();
    public RigidBody Right = new();
    public RigidBody Up = new();

    public void Create(float wallThickness, SKSize size)
    {
        _bounds = SKRect.Create(size);
        var up = new RigidBody
        {
            Position = new Vector2(0, -wallThickness),
            Size = new SKSize(size.Width, wallThickness),
            Locked = true,
            CanBeRayCasted = false
        };
        var down = new RigidBody
        {
            Position = new Vector2(0, size.Height),
            Size = new SKSize(size.Width, wallThickness),
            Locked = true,
            CanBeRayCasted = false
        };
        var left = new RigidBody
        {
            Position = new Vector2(-wallThickness, 0),
            Size = new SKSize(wallThickness, size.Height),
            Locked = true,
            CanBeRayCasted = false
        };
        var right = new RigidBody
        {
            Position = new Vector2(size.Width, 0),
            Size = new SKSize(wallThickness, size.Height),
            Locked = true,
            CanBeRayCasted = false
        };
        Up = up;
        Down = down;
        Left = left;
        Right = right;
    }

    public bool Contains(SKRect rect)
    {
        return _bounds.Contains(rect);
    }

    public void UpdateSize(float wallThickness, SKSize size)
    {
        Up.Size = new SKSize(size.Width, wallThickness);
        Down.Size = new SKSize(size.Width, wallThickness);
        Left.Size = new SKSize(wallThickness, size.Height);
        Right.Size = new SKSize(wallThickness, size.Height);
    }
}