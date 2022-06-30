using System.Numerics;
using SkiaGame.Events;
using SkiaGame.Input;
using SkiaGame.Physics;
using SkiaSharp;

// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable MemberCanBePrivate.Global

namespace SkiaGame;

public abstract class Engine
{
    //Lista de corpos para desenho
    private readonly List<GameObject> _drawQueue = new();
    private readonly PhysicsEngine _physicsEngine;


    //Ultima vez em que o tempo foi medido para desenho
    private DateTime _lastTime = DateTime.Now;

    protected Engine()
    {
        _physicsEngine = new PhysicsEngine
        {
            OnPhysicsUpdate = OnPhysicsUpdate
        };
        TouchKeys = new TouchKeys();
    }

    public TouchKeys TouchKeys { get; }
    public bool DrawTouchKeys { get; set; } = true;

    public Mouse Mouse { get; } = new();

    /// <summary>
    ///     Obtem o Tamanho da tela
    /// </summary>
    public SKSize ScreenSize { get; private set; }

    /// <summary>
    ///     Taxa de Quadros por segundo
    /// </summary>
    public int FrameRate { get; set; } = 60;

    /// <summary>
    ///     Espaço de tempo em que a fisica ocorre, quanto menor o tempo, mais precisa, e mais custosa. Default:30ms
    /// </summary>
    public int PhysicsTimeStep
    {
        get => _physicsEngine.PhysicsTimeStep;
        set => _physicsEngine.PhysicsTimeStep = value;
    }

    /// <summary>
    ///     Essa é a cor em que a tela é limpa antes de desenhar os objetos
    /// </summary>
    public SKColor CLearColor { get; set; } = SKColors.White;

    /// <summary>
    ///     Aceleração da Gravidade : Default 9.81m/s²
    /// </summary>
    public float Gravity
    {
        get => _physicsEngine.Gravity;
        set => _physicsEngine.Gravity = value;
    }

    /// <summary>
    ///     Evento que Ocorre quando uma tecla virtual é pressionada ou solta
    /// </summary>
    public event EventHandler<TouchKeyEventArgs> OnTouchKeyChanged = (_, _) =>
    {
    };

    private void InitObjToEngine(GameObject gameObject)
    {
        if (gameObject.Engine != null) return;
        gameObject.Engine = this;
    }

    /// <summary>
    ///     Adiciona objeto na Engine Grafica e Fisica veja <see cref="AddPhysics" /> e <see cref="AddToDrawQueue" />
    ///     para adicionar separadamente>/>
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddToEngine(GameObject gameObject)
    {
        AddPhysics(gameObject);
        AddToDrawQueue(gameObject);
    }

    /// <summary>
    ///     Adiciona um Objeto à fisica, somente após adiciona-lo a física age no mesmo.
    /// </summary>
    /// <param name="gameObject">Objeto a ser adicionado</param>
    public void AddPhysics(GameObject gameObject)
    {
        InitObjToEngine(gameObject);
        _physicsEngine.AddBody(gameObject.RigidBody);
    }

    /// <summary>
    ///     Adiciona um Objeto para ser Desenhado
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddToDrawQueue(GameObject gameObject)
    {
        InitObjToEngine(gameObject);
        lock (_drawQueue)
        {
            if (_drawQueue.Contains(gameObject))
            {
                Console.WriteLine(
                    "Tentando ReAdicionar objeto a Lista de Desenho!!");
                return;
            }

            _drawQueue.Add(gameObject);
        }
    }

    /// <summary>
    ///     Para uso interno da plataforma, não deveria ser usado por enquanto pois não tem efeito em redimensionamento
    /// </summary>
    /// <param name="size"></param>
    public void InternalSetScreenSize(SKSize size)
    {
        ScreenSize = size;
    }

    /// <summary>
    ///     Para uso interno da plataforma
    /// </summary>
    public void InternalExecuteOnStart()
    {
        OnStart();
    }

    public void InternalSetMouseState(MouseInfo info)
    {
        Mouse.MouseState[info.Button] = info;
    }

    public void InternalTouchPress(SkTouchEventArgs args)
    {
        EventTouchKey key = TouchKeys.VerifyTouchCollision(args.Position);
        if (key != EventTouchKey.None)
        {
            Console.WriteLine($"Tecla {key} pressionada");
            OnTouchKeyChanged.Invoke(this,
                new TouchKeyEventArgs(key, TouchKeyEventType.Press));
        }

        Console.WriteLine($"Touch em {args.Position.X},{args.Position.Y}");
    }

    public void InternalTouchRelease(SkTouchEventArgs args)
    {
        var key = TouchKeys.VerifyTouchCollision(args.Position);
        if (key != EventTouchKey.None)
        {
            Console.WriteLine($"Tecla {key} soltada");
            OnTouchKeyChanged.Invoke(this,
                new TouchKeyEventArgs(key, TouchKeyEventType.Release));
        }

        Console.WriteLine(
            $"Touch Release em {args.Position.X},{args.Position.Y}");
    }

    public void InternalKeyPress(SkKeyPressEventArgs args)
    {
        Console.WriteLine($"Tecla {args.KeyCode} pressionada");
    }


    /// <summary>
    ///     Este é o evento onde os objetos são desenhados na tela.
    /// </summary>
    /// <param name="e"></param>
    public void OnPaintSurface(PaintEventArgs e)
    {
        ScreenSize = new SKSize(e.Info.Width, e.Info.Height);
        e.Surface.Canvas.Clear(CLearColor);
        var timeStep = (float)(DateTime.Now - _lastTime).TotalMilliseconds /
                       1000.0f;
        OnUpdate(e, timeStep);
        if (DrawTouchKeys)
        {
            TouchKeys.DrawFromCenter(e.Surface.Canvas,
                new Vector2(120, e.Info.Height - 120));
        }

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
    ///     Esta função é chamada sempre que o jogo é iniciado.
    /// </summary>
    protected abstract void OnStart();

    /// <summary>
    ///     Esta função é chamada a cada Frame de Desenho
    /// </summary>
    /// <param name="e">Parametros de evento</param>
    /// <param name="timeStep">Tempo entre o quadro anterior e este</param>
    protected abstract void OnUpdate(PaintEventArgs e, float timeStep);

    /// <summary>
    ///     Esta função é chamada a cada chamada da física
    /// </summary>
    /// <param name="timeStep">Tempo entre as chamadas</param>
    protected virtual void OnPhysicsUpdate(float timeStep)
    {
    }
}