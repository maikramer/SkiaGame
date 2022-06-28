using System.Numerics;
using SkiaGame.Physics;
using SkiaSharp;

namespace SkiaGame;

public abstract class Engine
{
    /// <summary>
    /// Taxa de Quadros por segundo
    /// </summary>
    public int FrameRate { get; set; } = 60;
    /// <summary>
    /// Espaço de tempo em que a fisica ocorre, quanto menor o tempo, mais precisa, e mais custosa.
    /// </summary>
    public int PhysicsTimeStep { get; set; } = 30;
    /// <summary>
    /// Essa é a cor em que a tela é limpa antes de desenhar os objetos
    /// </summary>
    public SKColor CLearColor { get; set; } = SKColors.White;
    /// <summary>
    /// Aceleração da Gravidade
    /// </summary>
    public float Gravity { get; set; } = 9.81f;
    //Ultima vez em que o tempo foi medido para desenho
    private DateTime _lastTime = DateTime.Now;
    //Ultima vez em que o tempo foi medido para a fisica
    private DateTime _physicsLastTime = DateTime.Now;
    //Lista de corpos para a fisica
    private readonly List<RigidBody> _bodies = new();
    //Lista de corpos para desenho
    private readonly List<GameObject> _drawQueue = new();

    protected Engine()
    {
        Task.Run(PhysicsEngine);
        Task.Run(OnStart);
    }
    /// <summary>
    /// Adiciona um Objeto à fisica, somente após adiciona-lo a física age no mesmo.
    /// </summary>
    /// <param name="gameObject">Objeto a ser adicionado</param>
    public void AddPhysics(GameObject gameObject)
    {
        lock (_bodies)
        {
            _bodies.Add(gameObject.RigidBody);
        }
    }
    /// <summary>
    /// Adiciona um Objeto para ser Desenhado
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddToDrawQueue(GameObject gameObject)
    {
        lock (_drawQueue)
        {
            _drawQueue.Add(gameObject);
        }
    }
    //Tarefa da Engine Fisica
    private async Task PhysicsEngine()
    {
        for (;;)
        {
            var timeStep = (float)((DateTime.Now - _physicsLastTime).TotalMilliseconds) / 1000.0f;
            lock (_bodies)
            {
                foreach (var body in _bodies)
                {
                    if (!body.ReactToCollision) continue;
                    var collided = false;
                    if (_bodies.Where(other => other != body).Any(other => body.Bounds.IntersectsWith(other.Bounds)))
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
    /// <summary>
    /// Este é o evento onde os objetos são desenhados na tela.
    /// </summary>
    /// <param name="e"></param>
    public void OnPaintSurface(PaintEventArgs e)
    {
        e.Surface.Canvas.Clear(CLearColor);
        var timeStep = (float)((DateTime.Now - _lastTime).TotalMilliseconds) / 1000.0f;
        OnUpdate(e, timeStep);
        lock (_drawQueue)
        {
            foreach (var gameObject in _drawQueue)
            {
                gameObject.Draw(e.Surface.Canvas);
            }
        }

        _lastTime = DateTime.Now;
    }

    /// <summary>
    /// Esta função é chamada a cada Frame de Desenho
    /// </summary>
    /// <param name="e">Parametros de evento</param>
    /// <param name="timeStep">Tempo entre o quadro anterior e este</param>
    protected abstract void OnUpdate(PaintEventArgs e, float timeStep);
    /// <summary>
    /// Esta função é chamada a cada chamada da física
    /// </summary>
    /// <param name="timeStep">Tempo entre as chamadas</param>
    protected abstract void OnPhysicsUpdate(float timeStep);
    /// <summary>
    /// Esta função é chamada sempre que o jogo é iniciado.
    /// </summary>
    protected abstract void OnStart();
}