using System.Numerics;
using System.Drawing;
using SkiaGame.Physics.Classes;
using SkiaGame.Physics.Helpers;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace SkiaGame.Physics;

public class PhysicsEngine
{
    private const int OutOfBoundsValue = 5000;

    /// <summary>
    /// Escala em que a gravidade ocorre
    /// </summary>
    public float GravityScale { get; set; } = 1f;

    /// <summary>
    /// Escala em que a gravidade entre objetos ocorre
    /// </summary>
    public float BodiesGravityScale { get; set; } = 0.001f;

    /// <summary>
    /// Diz se a gravidade entre objetos está habilitada
    /// </summary>

    public bool BodiesGravityEnabled { get; set; } = false;

    /// <summary>
    /// Tempo entre frames de física dado em ms
    /// </summary>
    public int PhysicsTimeStep { get; set; } = 30;

    /// <summary>
    /// Aceleração da gravidade dada em pixels/s²
    /// </summary>
    public Vector2 Gravity { get; set; } = Vector2.UnitY * 120f;
    //Acumulador utilizado na fisica
    private double _accumulator;
    //Tempo entre frames dado em segundos
    private float SecondsTimeStep => PhysicsTimeStep / 1000f;
    //Acumulador máximo
    private float MaxAccumulator => SecondsTimeStep * 2;

    //Ultima vez em que o tempo foi medido para a fisica
    private DateTime _physicsLastTime = DateTime.Now;
    //Lista de pares de possíveis colisões
    private readonly List<CollisionPair> _listCollisionPairs = new();
    //Lista de objetos que atuam na gravidade
    private readonly List<RigidBody?> _listGravityObjects = new();
    //Lista de objetos dentro da fisica
    private readonly List<RigidBody?> _listStaticObjects = new();
    /// <summary>
    /// Evento que acontece a cada frame de fisica
    /// </summary>
    public event Action<float> OnPhysicsUpdate = _ => { };

    internal PhysicsEngine()
    {
        Task.Run(PhysicsTask);
    }

    public void AddBody(RigidBody rigidBody)
    {
        lock (_listStaticObjects)
        {
            _listStaticObjects.Add(rigidBody);
        }

        lock (_listGravityObjects)
        {
            _listGravityObjects.Add(rigidBody);
        }
    }

    private async Task PhysicsTask()
    {
        for (;;)
        {
            var timeStep =
                (float)(DateTime.Now - _physicsLastTime).TotalMilliseconds /
                1000.0f;
            Tick(timeStep);
            OnPhysicsUpdate.Invoke(timeStep);
            _physicsLastTime = DateTime.Now;
            await Task.Delay(PhysicsTimeStep);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private IEnumerable<RigidBody?> GetMovableObjects()
    {
        for (var i = _listStaticObjects.Count - 1; i >= 0; i--)
        {
            var obj = _listStaticObjects[i];
            if (obj == null) continue;
            if (!obj.Locked && obj.Mass < 1000000)
            {
                yield return obj;
            }
        }
    }

    public static readonly Queue<RigidBody?> RemovalQueue = new();

    public void FreezeStaticObjects()
    {
        foreach (var rigidBody in _listStaticObjects)
        {
            if (rigidBody == null) continue;
            rigidBody.Velocity = new Vector2
            {
                X = 0,
                Y = 0
            };
        }
    }

    public void RemoveAllMovableObjects()
    {
        foreach (var obj in GetMovableObjects())
        {
            RemovalQueue.Enqueue(obj);
        }
    }

    public void Tick(double elapsedTime)
    {
        _accumulator += elapsedTime;

        //Avoid accumulator spiral of death by clamping
        if (_accumulator > MaxAccumulator)
            _accumulator = MaxAccumulator;

        while (_accumulator > SecondsTimeStep)
        {
            BroadPhaseGeneratePairs();
            UpdatePhysics(SecondsTimeStep);
            ProcessRemovalQueue();
            _accumulator -= SecondsTimeStep;
        }
    }


    private Vector2 Attenuate(Vector2 vector, float factor)
    {
        float At(float v)
        {
            return v > 0
                ? v - factor
                : v < 0
                    ? v - factor
                    : 0;
        }

        return new Vector2(At(vector.X), At(vector.Y));
    }

    private void ApplyConstants(RigidBody? body, float dt)
    {
        if (body == null) return;
        if (body.Locked)
        {
            return;
        }

        AddGravity(body, dt);
        AddFriction(body, dt);
    }

    private void AddGravity(RigidBody? body, float dt)
    {
        if (body == null) return;
        if (body.HasGravity)
        {
            body.Velocity += GetGravityVector(body) * dt;
        }
    }

    private void AddFriction(RigidBody body, float delta)
    {
        body.Velocity = Attenuate(body.Velocity, body.Friction * delta);
    }

    private Vector2 CalculatePointGravity(RigidBody? body)
    {
        var forces = new Vector2(0, 0);
        if (body == null) return Vector2.Zero;
        if (body.Locked)
        {
            return forces;
        }

        foreach (var rigidBody in _listGravityObjects)
        {
            if (rigidBody is not { GeneratesGravity: true }) continue;
            var diff = rigidBody.Center - body.Center;
            PhysMath.RoundToZero(ref diff, 5F);

            //apply inverse square law
            float falloffMultiplier = 0;
            if (diff.LengthSquared() != 0)
            {
                falloffMultiplier = rigidBody.Mass / diff.LengthSquared();
            }

            diff.X = (int)diff.X == 0 ? 0 : diff.X * falloffMultiplier;
            diff.Y = (int)diff.Y == 0 ? 0 : diff.Y * falloffMultiplier;

            if (diff.Length() > .005F)
            {
                forces += diff;
            }
        }

        return forces;
    }

    private RigidBody? RaytraceAtPoint(PointF p)
    {
        return _listStaticObjects.FirstOrDefault(body => body != null && body.Contains(p));
    }

    private Vector2 GetGravityVector(RigidBody? obj)
    {
        return CalculatePointGravity(obj) * BodiesGravityScale + Gravity * GravityScale;
    }

    private void ProcessRemovalQueue()
    {
        if (RemovalQueue.Count > 0)
        {
            var obj = RemovalQueue.Dequeue();
            _listStaticObjects.Remove(obj);
            _listGravityObjects.Remove(obj);
        }
    }

    private void UpdatePhysics(float dt)
    {
        foreach (var pair in _listCollisionPairs)
        {
            var objA = pair.A;
            var objB = pair.B;

            var m = new Manifold();
            var collision = false;

            if (objA.ShapeType == RigidBody.Type.Circle && objB.ShapeType == RigidBody.Type.Box)
            {
                m.A = objB;
                m.B = objA;
            }
            else
            {
                m.A = objA;
                m.B = objB;
            }

            //Box vs anything
            if (m.A.ShapeType == RigidBody.Type.Box)
            {
                if (m.B.ShapeType == RigidBody.Type.Box)
                {
                    //continue;
                    if (Collision.AabbVsAabb(ref m))
                    {
                        collision = true;
                    }
                }

                if (m.B == null) continue;
                if (m.B.ShapeType == RigidBody.Type.Circle)
                {
                    if (Collision.AabBvsCircle(ref m))
                    {
                        collision = true;
                    }
                }
            }

            //Circle Circle
            else
            {
                if (m.B.ShapeType == RigidBody.Type.Circle)
                {
                    if (Collision.CircleVsCircle(ref m))
                    {
                        collision = true;
                    }
                }
            }

            //Resolve Collision
            if (collision)
            {
                Collision.ResolveCollision(ref m);
                Collision.PositionalCorrection(ref m);
                objA.LastCollision = m;
                objB.LastCollision = m;
            }
        }

        foreach (var body in _listStaticObjects)
        {
            if (body == null) continue;
            ApplyConstants(body, dt);
            body.Move(dt);
            DetectOutOfBounds(body);
        }
    }

    private void DetectOutOfBounds(RigidBody body)
    {
        if (body.Center.Y is > OutOfBoundsValue or < -OutOfBoundsValue ||
            body.Center.X is > OutOfBoundsValue or < -OutOfBoundsValue)
        {
            RemovalQueue.Enqueue(body);
        }
    }

    private void BroadPhaseGeneratePairs()
    {
        _listCollisionPairs.Clear();

        for (var i = 0; i < _listStaticObjects.Count; i++)
        {
            for (var j = i; j < _listStaticObjects.Count; j++)
            {
                if (j == i)
                {
                    continue;
                }

                var a = _listStaticObjects[i];
                var b = _listStaticObjects[j];
                if (a == null || b == null) continue;

                var aBb = a.Aabb;
                var aabb = b.Aabb;

                if (Collision.AabbVsAabb(aBb, aabb))
                {
                    _listCollisionPairs.Add(new CollisionPair(a, b));
                }
            }
        }
    }
}