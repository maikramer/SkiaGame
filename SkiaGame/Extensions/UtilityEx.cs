using System.Numerics;
using SkiaSharp;

namespace SkiaGame.Extensions;

public static class Extensions
{
    public static Vector2 ToVector2(this SKPoint point)
    {
        return new Vector2(point.X, point.Y);
    }

    public static SKPoint ToSkPoint(this Vector2 vector2)
    {
        return new SKPoint(vector2.X, vector2.Y);
    }
}