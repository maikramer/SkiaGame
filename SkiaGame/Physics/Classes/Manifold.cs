using System.Numerics;

namespace SkiaGame.Physics.Classes
{
    public class Manifold
    {
        public RigidBody? A;
        public RigidBody? B;
        public float Penetration;
        public Vector2 Normal;
    }
}