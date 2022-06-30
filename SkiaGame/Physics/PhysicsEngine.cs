using System.Numerics;

namespace SkiaGame.Physics;

internal class PhysicsEngine
{
    public int PhysicsTimeStep { get; set; } = 30;
    public float Gravity { get; set; } = 9.81f;

    internal PhysicsEngine()
    {
        Task.Run(PhysicsTask);
    }

    //Ultima vez em que o tempo foi medido para a fisica
    private DateTime _physicsLastTime = DateTime.Now;

    //Lista de corpos para a fisica
    private readonly List<RigidBody> _bodies = new();
    //Tarefa da Engine Fisica

    public Action<float> OnPhysicsUpdate = _ => { };

    public void AddBody(RigidBody rigidBody)
    {
        
        lock (_bodies)
        {
            if (_bodies.Contains(rigidBody))
            {
                Console.WriteLine("Tentando ReAdicionar objeto a fisica!!");
                return;
            }

            _bodies.Add(rigidBody);
        }
    }

    private async Task PhysicsTask()
    {
        for (;;)
        {
            var timeStep = (float)((DateTime.Now - _physicsLastTime).TotalMilliseconds) / 1000.0f;
            lock (_bodies)
            {
                foreach (var body in _bodies)
                {
                    if (!body.ReactToCollision && !body.HasGravity) continue;
                    var collided = false;
                    if (body.ReactToCollision &&
                        _bodies.Where(other => other != body).Any(other => body.Bounds.IntersectsWith(other.Bounds)))
                    {
                        collided = true;
                        body.Speed = -body.Elasticity * body.Speed;
                        body.Update(timeStep);
                    }

                    if (collided) break;
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
}