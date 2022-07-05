using System.Numerics;
using SkiaGame;
using SkiaGame.Events;
using SkiaGame.Input;
using SkiaGame.Physics;
using SkiaSharp;

namespace TestGame;

public class PhysicsTester : Engine
{
    private const float GroundHeight = 30;
    private static float _charDiameter = 20;
    private RigidBody? _holdingBody;
    private bool _lastMouseState;
    private Vector2 _clickPoint;
    private GameObject _char = new();

    private readonly GameObject _ground = new()
    {
        Primitive = Primitive.Rect, Locked = true, Color = SKColors.Peru
    };

    /// <summary>
    ///     Começa depois de 3 Segundos
    /// </summary>
    private readonly TimeSpan _startTime = TimeSpan.FromSeconds(3);

    private bool _notStarted = true;

    private void CreateAndPopulateBalls()
    {
        var rand = new Random(DateTime.Now.Millisecond);
        for (var i = 1; i < 14; i++)
        for (var j = 1; j < 14; j++)
        {
            float calcX = 0;
            while (calcX <= 0 || calcX >= ScreenInfo.Size.Width - _charDiameter)
                calcX = rand.NextSingle() * ScreenInfo.Size.Width - _charDiameter / 2;

            float calcY = 0;
            while (calcY <= 0 || calcY >= ScreenInfo.Size.Height - _charDiameter)
                calcY = rand.NextSingle() * ScreenInfo.Size.Height - _charDiameter / 2;

            var ball = new GameObject
            {
                Primitive = Primitive.Circle,
                Diameter = _charDiameter,
                Color = SKColors.Crimson,
                Locked = false,
                Position = new Vector2(calcX, calcY),
                RigidBody = { Velocity = new Vector2(rand.Next(400), rand.Next(400)) }
            };

            AddToEngine(ball);
        }
    }

    protected override void OnStart()
    {
        _charDiameter *= ScreenInfo.Density;
        _char = new GameObject
        {
            Primitive = Primitive.Circle,
            Diameter = _charDiameter * 1.4f,
            Color = SKColors.Blue,
            Locked = false
        };
        PhysicsEngine.IsPaused = true;

        CreateAndPopulateBalls();
        _char.Position = new Vector2(100,
            ScreenInfo.Size.Height - _charDiameter - GroundHeight - 30);
        AddToEngine(_char);
        PhysicsEngine.CreateBoundingBox(ScreenInfo.Size);
        ScreenSizeChanged += OnScreenSizeChanged;
        ScreenOrientationChanged += (_, args) =>
        {
            Console.WriteLine($"Orientação mudou para {args.NewValue}");
        };
    }

    private void OnScreenSizeChanged(object? sender, ScreenSizeChangeEventArgs e)
    {
        PhysicsEngine.UpdateBoundingBox(e.NewValue);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
        if (_notStarted && TimeSinceStart > _startTime)
        {
            _notStarted = false;
            PhysicsEngine.IsPaused = false;
        }

        //Seta posição em Runtime, de acordo com que a pessoa altera o tamanho da tela
        _ground.Position = new Vector2(0, e.Info.Height - GroundHeight);
        _ground.Size = new SKSize(e.Info.Width, GroundHeight);
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
            //Primeiro click
            if (_lastMouseState == false)
            {
                _clickPoint = point;
            }

            _holdingBody = PhysicsEngine.Raycast(point);
            if (_holdingBody != null)
            {
                _holdingBody.Velocity = Vector2.Zero;
                _holdingBody.Locked = true;
                _holdingBody.Center = Mouse[MouseButton.Left].Position;
            }

            _lastMouseState = Mouse[MouseButton.Left].IsPressed;
        }

        //Soltou o botao do mouse ou touch
        if (_lastMouseState && _holdingBody != null)
        {
            _holdingBody.Locked = false;
            _clickPoint = Vector2.Zero;
            _lastMouseState = false;
        }
    }

    private void HandleKeyboardAndTouchKeys(float timeStep)
    {
        if (TouchKeys.Up.IsPressed || Keyboard[KeyCode.w].IsPressed ||
            Keyboard[KeyCode.W].IsPressed)
            _char.RigidBody.AddForce(-Vector2.UnitY, 1000, timeStep);
        else if (TouchKeys.Down.IsPressed || Keyboard[KeyCode.s].IsPressed ||
                 Keyboard[KeyCode.S].IsPressed)
            _char.RigidBody.AddForce(Vector2.UnitY, 1000, timeStep);
        else if (TouchKeys.Right.IsPressed || Keyboard[KeyCode.d].IsPressed ||
                 Keyboard[KeyCode.D].IsPressed)
            _char.RigidBody.AddForce(Vector2.UnitX, 1000, timeStep);
        else if (TouchKeys.Left.IsPressed || Keyboard[KeyCode.a].IsPressed ||
                 Keyboard[KeyCode.A].IsPressed)
            _char.RigidBody.AddForce(-Vector2.UnitX, 1000, timeStep);
    }
}