using System.Numerics;
using SkiaGame.Physics;

namespace SkiaGame;

public abstract class Engine
{
    public int FrameRate { get; set; } = 60;
    public int PhysicsTimeStep { get; set; } = 50;
    public float Gravity { get; set; } = 9.81f;
    private DateTime _lastTime = DateTime.Now;
    private DateTime _physicsLastTime = DateTime.Now;
    private readonly List<Body> _bodies = new();

    protected Engine()
    {
        Task.Run(PhysicsEngine);
        Task.Run(OnStart);
    }

    public void AddBody(Body body)
    {
        lock (_bodies)
        {
            _bodies.Add(body);
        }
    }

    private async Task PhysicsEngine()
    {
        for (;;)
        {
            var timeStep = (float)((DateTime.Now - _physicsLastTime).TotalMilliseconds) / 1000.0f;
            lock (_bodies)
            {
                foreach (var body in _bodies)
                {
                    body.Speed += Gravity * new Vector2(0, 1) * timeStep;
                    body.Update(timeStep);
                }
            }


            OnPhysicsUpdate(timeStep);

            _physicsLastTime = DateTime.Now;
            await Task.Delay(PhysicsTimeStep);
        }

        // ReSharper disable once FunctionNeverReturns
    }

    public void OnPaintSurface(PaintEventArgs e)
    {
        var timeStep = (float)((DateTime.Now - _lastTime).TotalMilliseconds) / 1000.0f;
        OnUpdate(e, timeStep);
        _lastTime = DateTime.Now;
    }

    protected abstract void OnUpdate(PaintEventArgs e, float timeStep);
    protected abstract void OnPhysicsUpdate(float timeStep);

    protected abstract void OnStart();
}