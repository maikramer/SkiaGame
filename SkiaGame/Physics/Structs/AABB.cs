using System.Numerics;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable InconsistentNaming

namespace SkiaGame.Physics.Structs
{
    /// <summary>
    /// Axis Aligned bounding box struct that represents the position of an object within a coordinate system.
    /// </summary>
    public struct AABB
    {
        public bool Equals(AABB other)
        {
            return Min.Equals(other.Min) && Max.Equals(other.Max);
        }

        public override bool Equals(object? obj)
        {
            return obj is AABB other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }

        public Vector2 Min = Vector2.One;
        public Vector2 Max = Vector2.One * 2;

        public AABB()
        {
        }

        public float Area => (Max.X - Min.X) * (Max.Y - Min.Y);

        public static bool operator ==(AABB left, AABB right)
        {
            return left.Min == right.Min && left.Max == right.Max;
        }

        public static bool operator !=(AABB left, AABB right)
        {
            return left.Min != right.Min || left.Max != right.Max;
        }
    }
}