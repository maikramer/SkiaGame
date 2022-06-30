using System.Numerics;

namespace SkiaGame.Physics;

internal class PhysicsEngine
{
    //Lista de corpos para a fisica
    private readonly List<RigidBody> _bodies = new();

    //Ultima vez em que o tempo foi medido para a fisica
    private DateTime _physicsLastTime = DateTime.Now;
    //Tarefa da Engine Fisica

    public event Action<float> OnPhysicsUpdate = _ => { };

    internal PhysicsEngine()
    {
        Task.Run(PhysicsTask);
    }

    public int PhysicsTimeStep { get; set; } = 30;
    public float Gravity { get; set; } = 9.81f;

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

    public void AddForce(Vector2 direction, float strength, GameObject gameObject, float timeStep)
    {
        var rigidBody = gameObject.RigidBody;
        var acc = strength / rigidBody.Mass;
        rigidBody.Speed += direction * acc * timeStep;
        rigidBody.Update(timeStep);
    }

    private async Task PhysicsTask()
    {
        for (;;)
        {
            var timeStep =
                (float)(DateTime.Now - _physicsLastTime).TotalMilliseconds /
                1000.0f;
            lock (_bodies)
            {
                foreach (var body in _bodies)
                {
                    if (!body.ReactToCollision && !body.HasGravity) continue;
                    var collided = false;
                    var hasCollisions = false;
                    RigidBody? otherRigidBody = null;
                    foreach (var other in _bodies)
                    {
                        if (other == body) continue;
                        if (!body.Bounds.IntersectsWith(other.Bounds)) continue;
                        hasCollisions = true;
                        otherRigidBody = other;
                        break;
                    }

                    if (body.ReactToCollision &&
                        hasCollisions)
                    {
                        collided = true;
                        if (otherRigidBody != null)
                        {
                            body.Speed = -Math.Sign((otherRigidBody.Center - body.Center).Y) *
                                         body.Elasticity * body.Speed;
                        }

                        body.Update(timeStep);
                    }

                    if (collided) break;
                    body.Speed += Gravity * Vector2.UnitY * timeStep;
                    body.Update(timeStep);
                }
            }


            OnPhysicsUpdate.Invoke(timeStep);

            _physicsLastTime = DateTime.Now;
            await Task.Delay(PhysicsTimeStep);
        }

        // ReSharper disable once FunctionNeverReturns
    }
}