using System.Numerics;

namespace SkiaGame.Physics.Classes;

public class Manifold
{
    public RigidBody? A;
    public RigidBody? B;
    public Vector2 Normal;
    public float Penetration;
}