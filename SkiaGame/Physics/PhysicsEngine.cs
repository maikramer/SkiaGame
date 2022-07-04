using System.Drawing;
using System.Numerics;
using SkiaGame.Physics.Classes;
using SkiaSharp;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace SkiaGame.Physics;

public class PhysicsEngine
{
    private const int OutOfBoundsValue = 5000;
    private const float MinimalGravityVelocity = 3f;
    private const float MinimalVelocity = 0.1f;

    public static readonly Queue<RigidBody?> RemovalQueue = new();

    private readonly BoundingBox _boundingBox = new();

    private readonly Engine _engine;

    //Lista de pares de possíveis colisões
    private readonly List<CollisionPair> _listCollisionPairs = new();

    //Lista de objetos que atuam na gravidade
    private readonly List<RigidBody?> _listGravityObjects = new();

    //Lista de objetos dentro da fisica
    private readonly List<RigidBody?> _listStaticObjects = new();

    private float _lastTimeScale;

    //Ultima vez em que o tempo foi medido para a fisica
    private DateTime _physicsLastTime = DateTime.Now;

    internal PhysicsEngine(Engine engine)
    {
        _engine = engine;
        Task.Run(PhysicsTask);
    }

    public float TimeScale { get; set; } = 1.0f;

    public bool IsPaused
    {
        set
        {
            if (value)
            {
                _lastTimeScale = TimeScale;
                TimeScale = 0;
            }
            else
            {
                TimeScale = _lastTimeScale;
            }
        }
        get => TimeScale == 0;
    }

    /// <summary>
    ///     Espessura da parede de contenção
    /// </summary>
    public float WallThickness { get; set; } = 50;

    /// <summary>
    ///     Escala em que a gravidade ocorre
    /// </summary>
    public float GravityScale { get; set; } = 1f;

    /// <summary>
    ///     Escala em que a gravidade entre objetos ocorre
    /// </summary>
    public float BodiesGravityScale { get; set; } = 0.001f;

    /// <summary>
    ///     Diz se a gravidade entre objetos está habilitada
    /// </summary>

    public bool BodiesGravityEnabled { get; set; } = false;

    /// <summary>
    ///     Tempo entre frames de física dado em ms
    /// </summary>
    public int PhysicsTimeStep { get; set; } = 15;

    /// <summary>
    ///     Aceleração da gravidade dada em pixels/s²
    /// </summary>
    public Vector2 Gravity { get; set; } = Vector2.UnitY * 150f;


    //Tempo entre frames dado em segundos
    private float SecondsTimeStep => PhysicsTimeStep / 1000f;

    //Acumulador máximo
    private float MaxAccumulator => SecondsTimeStep * 2;

    /// <summary>
    ///     Evento que acontece a cada frame de fisica
    /// </summary>
    public event Action<float> BeforePhysicsUpdate = _ => { };

    public event Action<float> AfterPhysicsUpdate = _ => { };

    /// <summary>
    ///     Atualiza o tamanho da caixa de contenção
    /// </summary>
    /// <param name="size"></param>
    public void UpdateBoundingBox(SKSize size)
    {
        lock (_listStaticObjects)
        {
            _listStaticObjects.Remove(_boundingBox.Up);
            _listStaticObjects.Remove(_boundingBox.Down);
            _listStaticObjects.Remove(_boundingBox.Left);
            _listStaticObjects.Remove(_boundingBox.Right);
        }

        CreateBoundingBox(size);
    }

    /// <summary>
    ///     Cria uma caixa de contenção utilizando o tamanho total da tela
    /// </summary>
    public void CreateBoundingBox()
    {
        CreateBoundingBox(_engine.ScreenInfo.Size);
    }

    /// <summary>
    ///     Cria uma caixa de contenção
    /// </summary>
    /// <param name="size"></param>
    public void CreateBoundingBox(SKSize size)
    {
        _boundingBox.Create(WallThickness, size);
        lock (_listStaticObjects)
        {
            _listStaticObjects.Add(_boundingBox.Up);
            _listStaticObjects.Add(_boundingBox.Down);
            _listStaticObjects.Add(_boundingBox.Left);
            _listStaticObjects.Add(_boundingBox.Right);
        }
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
            if (!IsPaused)
            {
                var deltaTime = (float)(DateTime.Now - _physicsLastTime).TotalMilliseconds /
                                1000.0f;
                deltaTime *= TimeScale;
                BeforePhysicsUpdate.Invoke(deltaTime);
                PhysicsTick(deltaTime);
                AfterPhysicsUpdate.Invoke(deltaTime);
            }

            _physicsLastTime = DateTime.Now;
            await Task.Delay(PhysicsTimeStep);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private IEnumerable<RigidBody?> GetMovableObjects()
    {
        lock (_listStaticObjects)
        {
            for (var i = _listStaticObjects.Count - 1; i >= 0; i--)
            {
                var obj = _listStaticObjects[i];
                if (obj == null) continue;
                if (!obj.Locked && obj.Mass < 1000000) yield return obj;
            }
        }
    }

    public void FreezeStaticObjects()
    {
        lock (_listStaticObjects)
        {
            foreach (var rigidBody in _listStaticObjects)
            {
                if (rigidBody == null) continue;
                lock (rigidBody)
                {
                    rigidBody.Velocity = new Vector2
                    {
                        X = 0,
                        Y = 0
                    };
                }
            }
        }
    }

    public void RemoveAllMovableObjects()
    {
        foreach (var obj in GetMovableObjects()) RemovalQueue.Enqueue(obj);
    }

    public void PhysicsTick(float deltaTime)
    {
        BroadPhaseGeneratePairs();
        UpdatePhysics(deltaTime);
        ProcessRemovalQueue();
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
        if (body.Locked) return;

        AddGravity(body, dt);
        AddFriction(body, dt);
    }

    private void AddGravity(RigidBody? body, float dt)
    {
        if (body == null) return;
        if (body.HasGravity) body.Velocity += GetGravityVector(body) * dt;
    }

    private void AddFriction(RigidBody body, float delta)
    {
        body.Velocity = Attenuate(body.Velocity, body.Friction * delta);
        if (body.Velocity.Length() < MinimalVelocity)
        {
            body.Velocity = Vector2.Zero;
        }
    }

    private Vector2 CalculatePointGravity(RigidBody? body)
    {
        var forces = new Vector2(0, 0);
        if (body == null) return Vector2.Zero;
        if (body.Locked) return forces;

        foreach (var rigidBody in _listGravityObjects)
        {
            if (rigidBody is not { GeneratesGravity: true }) continue;
            var diff = rigidBody.Center - body.Center;
            if (diff.Length() < MinimalGravityVelocity)
            {
                diff.X = 0;
                diff.Y = 0;
            }

            //apply inverse square law
            float falloffMultiplier = 0;
            if (diff.LengthSquared() != 0)
                falloffMultiplier = rigidBody.Mass / diff.LengthSquared();

            diff.X = (int)diff.X == 0 ? 0 : diff.X * falloffMultiplier;
            diff.Y = (int)diff.Y == 0 ? 0 : diff.Y * falloffMultiplier;

            if (diff.Length() > .005F) forces += diff;
        }

        return forces;
    }

    public RigidBody? Raycast(PointF p)
    {
        lock (_listStaticObjects)
        {
            return _listStaticObjects.FirstOrDefault(body =>
                body is { CanBeRayCasted: true } && body.Contains(p));
        }
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
            lock (_listStaticObjects)
            {
                _listStaticObjects.Remove(obj);
            }

            lock (_listGravityObjects)
            {
                _listGravityObjects.Remove(obj);
            }
        }
    }

    private void UpdatePhysics(float deltaTime)
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
                    //continue;
                    if (Collision.AabbVsAabb(ref m))
                        collision = true;

                if (m.B == null) continue;
                if (m.B.ShapeType == RigidBody.Type.Circle)
                    if (Collision.AabBvsCircle(ref m))
                        collision = true;
            }

            //Circle Circle
            else
            {
                if (m.B.ShapeType == RigidBody.Type.Circle)
                    if (Collision.CircleVsCircle(ref m))
                        collision = true;
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

        lock (_listStaticObjects)
        {
            foreach (var body in _listStaticObjects)
            {
                if (body == null) continue;
                lock (body)
                {
                    ApplyConstants(body, deltaTime);
                    body.Move(deltaTime);
                    DetectOutOfBounds(body);
                }
            }
        }
    }

    private void DetectOutOfBounds(RigidBody body)
    {
        if (body.Center.Y is > OutOfBoundsValue or < -OutOfBoundsValue ||
            body.Center.X is > OutOfBoundsValue or < -OutOfBoundsValue)
            RemovalQueue.Enqueue(body);
    }

    private void BroadPhaseGeneratePairs()
    {
        _listCollisionPairs.Clear();

        lock (_listStaticObjects)
        {
            for (var i = 0; i < _listStaticObjects.Count; i++)
            for (var j = i; j < _listStaticObjects.Count; j++)
            {
                if (j == i) continue;

                var a = _listStaticObjects[i];
                var b = _listStaticObjects[j];
                if (a == null || b == null) continue;

                var aBb = a.Aabb;
                var aabb = b.Aabb;

                if (Collision.AabbVsAabb(aBb, aabb))
                    _listCollisionPairs.Add(new CollisionPair(a, b));
            }
        }
    }
}