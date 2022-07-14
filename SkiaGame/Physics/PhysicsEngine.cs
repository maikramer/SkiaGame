using System.Numerics;
using SkiaGame.Physics.Classes;
using SkiaSharp;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace SkiaGame.Physics;

public struct Force
{
    public RigidBody Body;
    public Vector2 Direction;
    public float Strength;


    public Force(RigidBody body, Vector2 direction, float strength)
    {
        Body = body;
        Direction = direction;
        Strength = strength;
    }
}

public class PhysicsEngine
{
    private const int OutOfBoundsValue = 5000;
    private const float FrictionCompensation = 0.001f;
    public static readonly Queue<RigidBody?> RemovalQueue = new();

    private readonly BoundingBox _boundingBox = new();

    private readonly Engine _engine;

    private readonly Queue<Force> _forcesQueue = new();

    //Lista de pares de possíveis colisões
    private readonly List<CollisionPair> _listCollisionPairs = new();

    //Lista de objetos que atuam na gravidade
    private readonly List<RigidBody> _listGravityObjects = new();

    //Lista de objetos dentro da fisica
    private readonly List<RigidBody> _listStaticObjects = new();

    private float _lastTimeScale;

    //Ultima vez em que o tempo foi medido para a fisica
    private DateTime _physicsLastTime = DateTime.Now;

    internal PhysicsEngine(Engine engine)
    {
        _engine = engine;
        Task.Run(PhysicsTask);
    }

    /// <summary>
    ///     Aborta adição do corpo se detectada colisão na adição
    /// </summary>
    public bool DontAddIfCollisionDetected { get; set; } = false;

    /// <summary>
    ///     Gravidade mínima entre 2 objetos
    /// </summary>
    public float MinimalGravityVelocity { get; set; } = 1.0f;

    /// <summary>
    ///     Velocidade mínima de um objeto
    /// </summary>
    public float MinimalVelocity { get; set; } = 0.2f;

    /// <summary>
    ///     Escala de tempo em que a física ocorre, em que Zero significa o jogo em pausa
    /// </summary>
    public float TimeScale { get; set; } = 1.0f;

    /// <summary>
    ///     Define o jogo em pausa, efetivamente seta <see cref="TimeScale" /> para 0 e salva o timescale anterior
    /// </summary>
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
                if (_lastTimeScale == 0) return;
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
    public int PhysicsTimeStep { get; set; } = 10;

    /// <summary>
    ///     Aceleração da gravidade dada em pixels/s²
    /// </summary>
    public Vector2 Gravity { get; set; } = Vector2.UnitY * 200f;


    //Tempo entre frames dado em segundos
    private float SecondsTimeStep => PhysicsTimeStep / 1000f;

    //Acumulador máximo
    private float MaxAccumulator => SecondsTimeStep * 2;

    /// <summary>
    ///     Evento que acontece a cada frame de fisica
    /// </summary>
    public event Action<float> BeforePhysicsUpdate = _ => { };

    /// <summary>
    ///     Evento que acontece após o frame de física
    /// </summary>
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

    public bool IsInsideBounds(SKRect rect)
    {
        return _boundingBox.Contains(rect);
    }

    public bool AddBody(RigidBody rigidBody)
    {
        var isValid = false;
        lock (_listStaticObjects)
        {
            foreach (var body in _listStaticObjects)
            {
                if (!rigidBody.CollidesWith(body)) continue;
                isValid = true;
                break;
            }
        }

        if (!isValid && DontAddIfCollisionDetected) return false;
        lock (_listStaticObjects)
        {
            _listStaticObjects.Add(rigidBody);
        }

        lock (_listGravityObjects)
        {
            _listGravityObjects.Add(rigidBody);
        }

        return isValid;
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


    private static float Attenuate(float value, float factor)
    {
        if (factor == 0 || value == 0) return value;
        while (factor > Math.Abs(value))
        {
            factor /= 2;
        }

        value += Math.Sign(value) * -1 * factor;
        return value;
    }

    private static Vector2 Attenuate(Vector2 vector, float factor)
    {
        return new Vector2(Attenuate(vector.X, factor), Attenuate(vector.Y, factor));
    }

    private void ApplyConstants(RigidBody body, float dt)
    {
        if (body.Locked) return;
        AddGravity(body, dt);
        AddFriction(body, dt);
        if (body.Velocity.Length() < MinimalVelocity)
        {
            body.Velocity = Vector2.Zero;
        }
    }

    private void AddGravity(RigidBody? body, float dt)
    {
        if (body == null) return;
        if (body.HasGravity) body.Velocity += GetGravityVector(body) * dt;
    }

    private static void AddFriction(RigidBody body, float delta)
    {
        body.Velocity = Attenuate(body.Velocity,
            body.Friction * delta * body.Velocity.LengthSquared() * FrictionCompensation);
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

    public RigidBody? Raycast(Vector2 p)
    {
        lock (_listStaticObjects)
        {
            return _listStaticObjects.FirstOrDefault(body =>
                body is { CanBeRayCasted: true } && body.Contains(p));
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="direction">Direção em que a força será aplicada</param>
    /// <param name="strength">Força em KN</param>
    public void AddForce(GameObject gameObject, Vector2 direction, float strength)
    {
        AddForce(gameObject.RigidBody, direction, strength);
    }

    /// <summary>
    /// </summary>
    /// <param name="body"></param>
    /// <param name="direction"></param>
    /// <param name="strength">Força em KN</param>
    public void AddForce(RigidBody body, Vector2 direction, float strength)
    {
        var force = new Force(body, direction, strength * 1000);
        _forcesQueue.Enqueue(force);
    }

    private Vector2 GetGravityVector(RigidBody? obj)
    {
        return CalculatePointGravity(obj) * BodiesGravityScale + Gravity * GravityScale;
    }

    private void ProcessRemovalQueue()
    {
        if (RemovalQueue.Count <= 0) return;
        var obj = RemovalQueue.Dequeue();
        lock (_listStaticObjects)
        {
            if (obj != null) _listStaticObjects.Remove(obj);
        }

        lock (_listGravityObjects)
        {
            if (obj != null) _listGravityObjects.Remove(obj);
        }
    }

    private void UpdatePhysics(float deltaTime)
    {
        foreach (var pair in _listCollisionPairs)
        {
            var objA = pair.A;
            var objB = pair.B;

            var m = new Manifold();
            var collision = objA.CollidesWith(objB, ref m);
            //Resolve Collision
            if (collision)
            {
                Collision.ResolveCollision(ref m);
                Collision.PositionalCorrection(ref m);
                objA.LastCollision = m;
                objB.LastCollision = m;
            }
        }

        while (_forcesQueue.Count > 0)
        {
            var force = _forcesQueue.Dequeue();
            var body = force.Body;
            body.Velocity += force.Direction * force.Strength * body.MassInverse * deltaTime;
        }

        lock (_listStaticObjects)
        {
            foreach (var body in _listStaticObjects)
            {
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

                var aBb = a.Aabb;
                var aabb = b.Aabb;

                if (Collision.AabbVsAabb(aBb, aabb))
                    _listCollisionPairs.Add(new CollisionPair(a, b));
            }
        }
    }
}