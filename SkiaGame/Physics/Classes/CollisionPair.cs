namespace SkiaGame.Physics.Classes;

public class CollisionPair
{
    public readonly RigidBody A;
    public readonly RigidBody B;

    public CollisionPair(RigidBody a, RigidBody b)
    {
        A = a;
        B = b;
    }

    private bool Equals(CollisionPair other) { return A.Equals(other.A) && B.Equals(other.B); }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((CollisionPair)obj);
    }

    public override int GetHashCode() { return HashCode.Combine(A, B); }

    public static bool operator ==(CollisionPair left, CollisionPair right)
    {
        if ((left.A.Aabb.Min == right.A.Aabb.Min && left.A.Aabb.Max == right.A.Aabb.Max &&
             left.B.Aabb.Min == right.B.Aabb.Min && left.B.Aabb.Max == right.B.Aabb.Max) ||
            (left.A.Aabb.Min == right.B.Aabb.Min && left.A.Aabb.Max == right.B.Aabb.Max &&
             left.B.Aabb.Min == right.A.Aabb.Min && left.B.Aabb.Max == right.A.Aabb.Max))
            return true;

        return false;
    }

    public static bool operator !=(CollisionPair left, CollisionPair right)
    {
        if ((left.A.Aabb.Min == right.A.Aabb.Min && left.A.Aabb.Max == right.A.Aabb.Max &&
             left.B.Aabb.Min == right.B.Aabb.Min && left.B.Aabb.Max == right.B.Aabb.Max) ||
            (left.A.Aabb.Min == right.B.Aabb.Min && left.A.Aabb.Max == right.B.Aabb.Max &&
             left.B.Aabb.Min == right.A.Aabb.Min && left.B.Aabb.Max == right.A.Aabb.Max))
            return false;

        return true;
    }
}