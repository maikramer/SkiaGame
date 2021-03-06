using System.Diagnostics;
using System.Numerics;
using Newtonsoft.Json;
using SkiaGame.Events;
using SkiaGame.Extensions;
using SkiaGame.Info;
using SkiaGame.Input;
using SkiaGame.Physics;
using SkiaGame.UI;
using SkiaSharp;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable MemberCanBePrivate.Global

namespace SkiaGame;

public abstract class Engine
{
    

    //Lista de corpos para desenho
    private readonly List<GameObject> _drawQueue = new();
    private readonly DateTime _startTime = DateTime.Now;

    public readonly PhysicsEngine PhysicsEngine;

    private string _gameFolder = string.Empty;

    //Ultima vez em que o tempo foi medido para desenho
    private DateTime _lastTime = DateTime.Now;
    private bool _startExecuted;

    protected Engine()
    {
        JsonSerializerSettings.Error = JsonError;
        TouchKeys = new TouchKeys();
        PhysicsEngine = new PhysicsEngine(this);
        PhysicsEngine.BeforePhysicsUpdate += BeforePhysicsUpdate;
        PhysicsEngine.AfterPhysicsUpdate += AfterPhysicsUpdate;
    }

    private JsonSerializerSettings JsonSerializerSettings { get; } = new();

    /// <summary>
    /// Distancia, em % de largura e altura, em que as teclas touch ficam dos cantos
    /// </summary>
    public float TouchKeyDistanceFromCorner { get; set; } = 0.035f;
    /// <summary>
    ///     Menu Principal do jogo
    /// </summary>
    public Menu MainMenu { get; set; } = new();

    /// <summary>
    ///     Título do jogo, utilizado também como nome das pastas do jogo
    /// </summary>
    public string Title { get; init; } = "SkiaGame";

    /// <summary>
    ///     Informações sobre a plataforma
    /// </summary>
    public Platform Platform { get; } = new();

    /// <summary>
    ///     Teclado TouchScreen
    /// </summary>
    public TouchKeys TouchKeys { get; }

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
    public ScreenInfo ScreenInfo { get; internal set; } = ScreenInfo.Zero;

    /// <summary>
    ///     Taxa de quadros por segundo
    /// </summary>
    public int FrameRate { get; set; } = 60;

    /// <summary>
    ///     Taxa em que que o input é verificado em algumas plataformas como o GTK
    /// </summary>
    public int InputFeedRate { get; set; } = 250;

    /// <summary>
    ///     Essa é a cor em que a tela é limpa antes de desenhar os objetos
    /// </summary>
    public SKColor ClearColor { get; set; } = SKColors.White;

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

    public bool WriteObjToFile<T>(T obj, string fileName)
    {
        //Somente GTK por enquanto
        if (!Platform.IsGtk) return false;
        var path = Path.Join(_gameFolder, fileName + ".json");
        using var file = File.CreateText(path);
        var serializer = new JsonSerializer();
        serializer.Serialize(file, obj);
        return true;
    }

    public bool ReadObjFromFile<T>(out T? obj, string fileName)
    {
        //Somente GTK por enquanto
        obj = default;
        if (!Platform.IsGtk) return false;
        var path = Path.Join(_gameFolder, fileName + ".json");
        if (!File.Exists(path)) return false;
        var str = File.ReadAllText(path);
        obj = JsonConvert.DeserializeObject<T>(str, JsonSerializerSettings);
        return true;
    }

    /// <summary>
    ///     Para uso interno da plataforma, seta as caracteristicas basicas da tela
    /// </summary>
    /// <param name="screenInfo"></param>
    public void InternalSetScreenInfo(ScreenInfo screenInfo)
    {
        if (!Equals(ScreenInfo, ScreenInfo.Zero)) return;
        TouchKeys.Resize(screenInfo.Density);
        MainMenu.UpdateSizes(screenInfo.Density);
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
    }

    /// <summary>
    ///     Para uso interno da plataforma
    /// </summary>
    /// <param name="mouseBase"></param>
    public void InternalSetMouseState(MouseBase mouseBase)
    {
        Mouse[mouseBase.Button] = mouseBase;
    }

    /// <summary>
    ///     Para uso interno da plataforma
    /// </summary>
    /// <param name="args"></param>
    public void InternalTouchPress(SkTouchEventArgs args)
    {
        VerifyTouchClick(args, true, TouchKeyEventType.Press);
    }

    /// <summary>
    ///     Para uso interno da plataforma
    /// </summary>
    /// <param name="args"></param>
    public void InternalTouchRelease(SkTouchEventArgs args)
    {
        VerifyTouchClick(args, false, TouchKeyEventType.Release);
    }

    private void VerifyTouchClick(SkTouchEventArgs args, bool isPress, TouchKeyEventType eventType)
    {
        var key = TouchKeys.VerifyTouchCollision(args.Position, isPress);
        if (key != TouchKeyEventCode.None)
        {
            TouchKeyChanged.Invoke(this, new TouchKeyEventArgs(key, eventType));
        }

        MainMenu.VerifyClick(args.Position.ToSkPoint(), isPress);
    }

    /// <summary>
    ///     Para uso interno da plataforma
    /// </summary>
    /// <param name="args"></param>
    public void InternalKeyPress(SkKeyPressEventArgs args)
    {
        if (args.KeyCode == KeyCode.None) return;
        Keyboard[args.KeyCode] = new KeyBase(true);
    }

    /// <summary>
    ///     Para uso interno da plataforma
    /// </summary>
    /// <param name="args"></param>
    public void InternalKeyRelease(SkKeyPressEventArgs args)
    {
        if (args.KeyCode == KeyCode.None) return;
        Keyboard[args.KeyCode] = new KeyBase(false);
    }

    /// <summary>
    ///     Para uso interno da plataforma
    /// </summary>
    /// <param name="position"></param>
    public void InternalUpdateMouseDesktop(Vector2 position)
    {
        Mouse.UpdatePosition(position);
        MainMenu.VerifyClick(position.ToSkPoint(), false);
    }

    /// <summary>
    ///     Para uso interno da plataforma
    /// </summary>
    /// <param name="gameFolder"></param>
    public void InternalSetGameFolder(string gameFolder)
    {
        if (!Directory.Exists(gameFolder))
        {
            Directory.CreateDirectory(gameFolder);
        }

        _gameFolder = gameFolder;
    }

    /// <summary>
    ///     Para uso interno da plataforma
    /// </summary>
    /// <param name="e"></param>
    public void InternalPaintSurface(PaintEventArgs e)
    {
        UpdateScreenInfo(e);

        e.Surface.Canvas.Clear(ClearColor);
        var timeStep = (float)(DateTime.Now - _lastTime).TotalMilliseconds / 1000.0f;
        OnUpdate(e, timeStep);
        lock (_drawQueue)
        {
            foreach (var gameObject in _drawQueue) gameObject.Draw(e.Surface.Canvas);
        }

        if (TouchKeys.Enabled)
        {
            var height = e.Info.Height;
            TouchKeys.DrawLeftBottom(e.Surface.Canvas,
                new Vector2(TouchKeyDistanceFromCorner * e.Info.Width, height - TouchKeyDistanceFromCorner * height));
        }

        if (MainMenu.Enabled)
        {
            MainMenu.Draw(e);
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
            ScreenInfo = new ScreenInfo(e.Info.Size, orientation, ScreenInfo.Density);
            TouchKeys.Resize(ScreenInfo.Density);
            MainMenu.UpdateSizes(ScreenInfo.Density);
            ScreenSizeChanged.Invoke(this, new ScreenSizeChangeEventArgs(oldSize, e.Info.Size));
            if (orientation != oldOrientation)
                ScreenOrientationChanged.Invoke(this,
                    new ScreenOrientationChangeEventArgs(oldOrientation, orientation));
        }
    }

    /// <summary>
    ///     Esta função pode ser sobrecarregada para lidar com erros de serialização de arquivos.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual void JsonError(object? sender, ErrorEventArgs e)
    {
        Debug.WriteLine($"Erro de Serialização com o objeto {e.CurrentObject}");
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
    ///     Esta função é chamada antes da física
    /// </summary>
    /// <param name="timeStep">Tempo entre as chamadas</param>
    protected virtual void BeforePhysicsUpdate(float timeStep)
    {
    }

    /// <summary>
    ///     Esta função é chamada depois da física
    /// </summary>
    /// <param name="timeStep">Tempo entre as chamadas</param>
    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual void AfterPhysicsUpdate(float timeStep)
    {
    }
}