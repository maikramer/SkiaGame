using System.Drawing;
using System.Numerics;
using SkiaGame.Physics.Structs;

namespace SkiaGame.Physics.Helpers;

internal static class PhysMath
{
    public static decimal DotProduct(PointF pa, PointF pb)
    {
        decimal[] a = { (decimal)pa.X, (decimal)pa.Y };
        decimal[] b = { (decimal)pb.X, (decimal)pb.Y };
        return a.Zip(b, (x, y) => x * y).Sum();
    }

    public static void CorrectBoundingBox(ref AABB aabb)
    {
        var p1 = new PointF(Math.Min(aabb.Min.X, aabb.Max.X), Math.Min(aabb.Min.Y, aabb.Max.Y));
        var p2 = new PointF(Math.Max(aabb.Min.X, aabb.Max.X), Math.Max(aabb.Min.Y, aabb.Max.Y));
        aabb.Min = new Vector2 { X = p1.X, Y = p1.Y };
        aabb.Max = new Vector2 { X = p2.X, Y = p2.Y };
    }

    public static void CorrectBoundingPoints(ref PointF p1, ref PointF p2)
    {
        var newP1 = new PointF(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
        var newP2 = new PointF(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));

        p1 = newP1;
        p2 = newP2;
    }

    public static void RoundToZero(ref Vector2 vector, float cutoff)
    {
        if (!(vector.Length() < cutoff)) return;
        vector.X = 0;
        vector.Y = 0;
    }

    public static float RadiansToDegrees(this float rads) { return (float)(180 / Math.PI) * rads; }
}