using System.Numerics;
using SkiaGame.Events;
using SkiaGame.Info;
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

    public readonly PhysicsEngine PhysicsEngine;

    //Ultima vez em que o tempo foi medido para desenho
    private DateTime _lastTime = DateTime.Now;
    private bool _startExecuted;
    private DateTime _startTime;

    protected Engine()
    {
        TouchKeys = new TouchKeys();
        PhysicsEngine = new PhysicsEngine(this);
        PhysicsEngine.BeforePhysicsUpdate += BeforePhysicsUpdate;
    }

    /// <summary>
    ///     Teclado TouchScreen
    /// </summary>
    public TouchKeys TouchKeys { get; }

    /// <summary>
    ///     Desenha ou não o teclado Touch na tela
    /// </summary>
    public bool DrawTouchKeys { get; set; } = true;

    /// <summary>
    ///     Tempo desde que o programa foi iniciado
    /// </summary>

    public TimeSpan TimeSinceStart => DateTime.Now - _startTime;

    /// <summary>
    ///     Propriedades de Mouse
    /// </summary>
    public Mouse Mouse { get; } = new();

    /// <summary>
    ///     Propriedades do Teclado físico
    /// </summary>
    public Keyboard Keyboard { get; } = new();

    /// <summary>
    ///     Obtém Informações sobre a tela
    /// </summary>
    /// <returns></returns>
    public ScreenInfo ScreenInfo { get; set; } = ScreenInfo.Zero;

    /// <summary>
    ///     Taxa de quadros por segundo
    /// </summary>
    public int FrameRate { get; set; } = 60;

    /// <summary>
    ///     Espaço de tempo em que a fisica ocorre, quanto menor o tempo, mais precisa, e mais custosa. Default:30ms
    /// </summary>
    public int PhysicsTimeStep
    {
        get => PhysicsEngine.PhysicsTimeStep;
        set => PhysicsEngine.PhysicsTimeStep = value;
    }

    /// <summary>
    ///     Essa é a cor em que a tela é limpa antes de desenhar os objetos
    /// </summary>
    public SKColor CLearColor { get; set; } = SKColors.White;

    /// <summary>
    ///     Evento que Ocorre quando uma tecla virtual é pressionada ou solta
    /// </summary>
    public event EventHandler<TouchKeyEventArgs> TouchKeyChanged = (_, _) => { };

    /// <summary>
    ///     Evento que ocorre quando o tamanho da tela muda
    /// </summary>
    public event EventHandler<ScreenSizeChangeEventArgs> ScreenSizeChanged = (_, _) => { };

    /// <summary>
    ///     Evento que ocorre quando a orientação da tela muda
    /// </summary>
    public event EventHandler<ScreenOrientationChangeEventArgs> ScreenOrientationChanged =
        (_, _) => { };

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
        PhysicsEngine.AddBody(gameObject.RigidBody);
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
                Console.WriteLine("Tentando Adicionar objeto que já existe na lista de Desenho!!");
                return;
            }

            _drawQueue.Add(gameObject);
        }
    }


    /// <summary>
    ///     Para uso interno da plataforma, seta as caracteristicas basicas da tela
    /// </summary>
    /// <param name="screenInfo"></param>
    public void InternalSetScreenInfo(ScreenInfo screenInfo)
    {
        if (!Equals(ScreenInfo, ScreenInfo.Zero)) return;
        ScreenInfo = screenInfo;
    }

    /// <summary>
    ///     Para uso interno da plataforma
    /// </summary>
    public void InternalExecuteOnStart()
    {
        if (_startExecuted) return;
        _startExecuted = true;
        OnStart();
        _startTime = DateTime.Now;
    }


    public void InternalSetMouseState(MouseInfo info) { Mouse[info.Button] = info; }

    public void InternalTouchPress(SkTouchEventArgs args)
    {
        var key = TouchKeys.VerifyTouchCollision(args.Position, true);
        if (key != TouchKeyEventCode.None)
        {
            Console.WriteLine($"Tecla {key} pressionada");
            TouchKeyChanged.Invoke(this, new TouchKeyEventArgs(key, TouchKeyEventType.Press));
        }

        Console.WriteLine($"Touch em {args.Position.X},{args.Position.Y}");
    }

    public void InternalTouchRelease(SkTouchEventArgs args)
    {
        var key = TouchKeys.VerifyTouchCollision(args.Position, false);
        if (key != TouchKeyEventCode.None)
        {
            Console.WriteLine($"Tecla {key} soltada");
            TouchKeyChanged.Invoke(this, new TouchKeyEventArgs(key, TouchKeyEventType.Release));
        }

        Console.WriteLine($"Touch Release em {args.Position.X},{args.Position.Y}");
    }

    public void InternalKeyPress(SkKeyPressEventArgs args)
    {
        if (args.KeyCode == KeyCode.None) return;
        Keyboard[args.KeyCode] = new KeyInfo(true);
    }

    public void InternalKeyRelease(SkKeyPressEventArgs args)
    {
        if (args.KeyCode == KeyCode.None || !Keyboard.ContainsKey(args.KeyCode)) return;
        Keyboard[args.KeyCode] = new KeyInfo(false);
    }


    /// <summary>
    ///     Este é o evento onde os objetos são desenhados na tela.
    /// </summary>
    /// <param name="e"></param>
    public void OnPaintSurface(PaintEventArgs e)
    {
        UpdateScreenInfo(e);

        e.Surface.Canvas.Clear(CLearColor);
        var timeStep = (float)(DateTime.Now - _lastTime).TotalMilliseconds / 1000.0f;
        OnUpdate(e, timeStep);
        if (DrawTouchKeys)
            TouchKeys.DrawFromCenter(e.Surface.Canvas, new Vector2(120, e.Info.Height - 120));

        lock (_drawQueue)
        {
            foreach (var gameObject in _drawQueue) gameObject.Draw(e.Surface.Canvas);
        }

        _lastTime = DateTime.Now;
    }

    private void UpdateScreenInfo(PaintEventArgs e)
    {
        if (Math.Abs(ScreenInfo.Size.Height - e.Info.Height) > 0.1f ||
            Math.Abs(ScreenInfo.Size.Width - e.Info.Width) > 0.1f)
        {
            var oldSize = ScreenInfo.Size;
            var orientation = e.Info.Size.Height > e.Info.Width
                ? Orientation.Portrait
                : Orientation.Landscape;
            var oldOrientation = ScreenInfo.Orientation;
            ScreenInfo = new ScreenInfo(e.Info.Size, orientation);
            ScreenSizeChanged.Invoke(this, new ScreenSizeChangeEventArgs(oldSize, e.Info.Size));
            if (orientation != oldOrientation)
                ScreenOrientationChanged.Invoke(this,
                    new ScreenOrientationChangeEventArgs(oldOrientation, orientation));
        }
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
    protected abstract void BeforePhysicsUpdate(float timeStep);
}