using System.Numerics;
using SkiaGame;
using SkiaGame.Events;
using SkiaGame.Input;
using SkiaGame.Physics;
using SkiaGame.UI;
using SkiaGame.UI.Settings;
using SkiaSharp;

//todo: Este exemplo já testa coisas demais, deve ser dividido.
namespace TestGame;

public class PhysicsStatus : FlipSetting
{
    public const string Running = "Rodando";
    public const string NotRunning = "Não Rodando";
    protected override string Positive => Running;
    protected override string Negative => NotRunning;
}

public struct Settings
{
    public PhysicsStatus PhysicsStatus;
    public int CharSize = 20;

    public Settings()
    {
        PhysicsStatus = new PhysicsStatus();
    }
}

public class PhysicsTester : Engine
{
    private readonly CircularSetting<int> _charSizeOptions =
        new(new[]
        {
            20, 30, 40, 50
        });

    public Settings Settings;
    private static float _ballsDiameter = 20;
    private RigidBody? _holdingBody;
    private RigidBody? _lastHoldingBody;
    private bool _lastMouseState;
    private GameObject _char = new();
    private MenuItem? _charSizeMenuItem;
    private MenuItem? _physicsStateMenuItem;

    protected PhysicsTester()
    {
        Title = "PhysicsTester";
    }

    private void CreateAndPopulateBalls()
    {
        var rand = new Random(DateTime.Now.Millisecond);
        for (var i = 1; i < 14; i++)
        for (var j = 1; j < 14; j++)
        {
            float calcX = 0;
            while (calcX <= 0 || calcX >= ScreenInfo.Size.Width - _ballsDiameter)
                calcX = rand.NextSingle() * ScreenInfo.Size.Width - _ballsDiameter / 2;

            float calcY = 0;
            while (calcY <= 0 || calcY >= ScreenInfo.Size.Height - _ballsDiameter)
                calcY = rand.NextSingle() * ScreenInfo.Size.Height - _ballsDiameter / 2;

            var ball = new GameObject
            {
                Primitive = Primitive.Circle,
                Diameter = _ballsDiameter,
                Color = SKColors.Crimson,
                Locked = false,
                Position = new Vector2(calcX, calcY),
                RigidBody =
                {
                    Velocity = new Vector2(rand.Next(400), rand.Next(400))
                }
            };

            AddToEngine(ball);
        }
    }

    protected override void OnStart()
    {
        _char = new GameObject
        {
            Primitive = Primitive.Circle,
            Diameter = _ballsDiameter * 1.4f,
            Color = SKColors.Blue,
            Locked = false
        };

        AddPhysicsStateMenuItem();
        AddCharSizeMenuItem();

        LoadSettings();
        _ballsDiameter *= ScreenInfo.Density;
        PhysicsEngine.IsPaused = Settings.PhysicsStatus.Status != PhysicsStatus.Running;
        CreateAndPopulateBalls();
        _char.Position = new Vector2(100,
            ScreenInfo.Size.Height - (Settings.CharSize));
        AddToEngine(_char);
        PhysicsEngine.CreateBoundingBox(ScreenInfo.Size);
        Mouse[MouseButton.Left].Pressed += () =>
        {
            Console.WriteLine("Botao Esquerdo Pressionado");
        };
        Mouse[MouseButton.RightButton].Released += () =>
        {
            Console.WriteLine("Botao Direito Soltado");
        };
        ScreenSizeChanged += OnScreenSizeChanged;
        ScreenOrientationChanged += (_, args) =>
        {
            Console.WriteLine($"Orientação mudou para {args.NewValue}");
        };
    }

    private void LoadSettings()
    {
        var res = ReadObjFromFile(out Settings settings, nameof(Settings));
        Settings = res ? settings : new Settings();
        _charSizeOptions.SetValue(Settings.CharSize);
        UpdateCharSize();
        UpdatePhysicsState();
    }

    private void SaveSettings()
    {
        Settings.CharSize = _charSizeOptions.Current;
        WriteObjToFile(Settings, nameof(Settings));
    }

    private void UpdateCharSize()
    {
        if (_charSizeMenuItem != null)
            _charSizeMenuItem.Text.Text = "Char Size: " + _charSizeOptions.Current;
        _char.Diameter = _charSizeOptions.Current * ScreenInfo.Density;
        SaveSettings();
    }

    private void AddCharSizeMenuItem()
    {
        _charSizeMenuItem = new MenuItem("Char Size: " + _charSizeOptions.Current);
        _charSizeMenuItem.Press += () =>
        {
            _charSizeOptions.Next();
            UpdateCharSize();
        };
        MainMenu.AddItem(_charSizeMenuItem);
    }

    private void UpdatePhysicsState()
    {
        if (_physicsStateMenuItem != null)
            _physicsStateMenuItem.Text.Text = "Physics: " + Settings.PhysicsStatus;
        PhysicsEngine.IsPaused = Settings.PhysicsStatus.Status != PhysicsStatus.Running;
        SaveSettings();
    }

    private void AddPhysicsStateMenuItem()
    {
        _physicsStateMenuItem = new MenuItem("Physics: " + Settings.PhysicsStatus);
        _physicsStateMenuItem.Press += () =>
        {
            Settings.PhysicsStatus.Flip();
            UpdatePhysicsState();
        };
        MainMenu.AddItem(_physicsStateMenuItem);
    }

    private void OnScreenSizeChanged(object? sender, ScreenSizeChangeEventArgs e)
    {
        PhysicsEngine.UpdateBoundingBox(e.NewValue);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
    }

    protected override void BeforePhysicsUpdate(float timeStep)
    {
        HandleMouseAndTouchOnBody();
        HandleKeyboardAndTouchKeys(timeStep);
    }

    private void HandleMouseAndTouchOnBody()
    {
        if (Mouse[MouseButton.Left].IsPressed)
        {
            var point = Mouse[MouseButton.Left].Position;
            _lastHoldingBody = _holdingBody;
            _holdingBody = PhysicsEngine.Raycast(point);
            if (_holdingBody != _lastHoldingBody && _lastHoldingBody != null)
            {
                _lastHoldingBody.Locked = false;
            }

            if (_holdingBody != null)
            {
                _holdingBody.Velocity = Vector2.Zero;
                _holdingBody.Locked = true;
                _holdingBody.Center = point;
            }

            _lastMouseState = Mouse[MouseButton.Left].IsPressed;
        }
        else //Soltou o botao do mouse ou touch
        if (_lastMouseState && _holdingBody != null)
        {
            _holdingBody.Locked = false;
            _lastMouseState = false;
        }
    }

    private void HandleKeyboardAndTouchKeys(float timeStep)
    {
        if (TouchKeys.Up.IsPressed || Keyboard[KeyCode.w].IsPressed ||
            Keyboard[KeyCode.W].IsPressed || Keyboard[KeyCode.Up].IsPressed)
            _char.RigidBody.AddForce(-Vector2.UnitY, 1000, timeStep);
        else if (TouchKeys.Down.IsPressed || Keyboard[KeyCode.s].IsPressed ||
                 Keyboard[KeyCode.S].IsPressed || Keyboard[KeyCode.Down].IsPressed)
            _char.RigidBody.AddForce(Vector2.UnitY, 1000, timeStep);
        else if (TouchKeys.Right.IsPressed || Keyboard[KeyCode.d].IsPressed ||
                 Keyboard[KeyCode.D].IsPressed || Keyboard[KeyCode.Right].IsPressed)
            _char.RigidBody.AddForce(Vector2.UnitX, 1000, timeStep);
        else if (TouchKeys.Left.IsPressed || Keyboard[KeyCode.a].IsPressed ||
                 Keyboard[KeyCode.A].IsPressed || Keyboard[KeyCode.Left].IsPressed)
            _char.RigidBody.AddForce(-Vector2.UnitX, 1000, timeStep);
    }
}