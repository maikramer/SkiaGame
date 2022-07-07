using System.Numerics;
using SkiaGame;
using SkiaGame.Events;
using SkiaGame.Helpers;
using SkiaGame.Input;
using SkiaGame.Physics;
using SkiaGame.UI;
using SkiaSharp;

//todo: Este exemplo já testa coisas demais, deve ser dividido.
namespace TestGame;

public class Settings
{
    public int CharSize = 30;
}

public enum PhysicsStatus
{
    Running,
    NotRunning
}

public class PhysicsTester : Engine
{
    private readonly CircularEnumerator<int> _charSizeOptions =
        new(new CircularList<int>
        {
            20,
            30,
            40,
            50
        });

    private readonly CircularEnumerator<PhysicsStatus> _physicsOptions =
        new(new CircularList<PhysicsStatus>
        {
            PhysicsStatus.NotRunning,
            PhysicsStatus.Running
        });

    public Settings Settings = new();
    private static float _charDiameter = 20;
    private RigidBody? _holdingBody;
    private bool _lastMouseState;
    private GameObject _char = new();

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
        AddPhysicsStateMenuItem();
        AddCharSizeMenuItem();
        SaveObjToFile(Settings, nameof(Settings));
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
            ScreenInfo.Size.Height - _charDiameter);
        AddToEngine(_char);
        PhysicsEngine.CreateBoundingBox(ScreenInfo.Size);
        Mouse.AddListenerToButton(MouseButton.Left,
            args =>
            {
                Console.WriteLine(
                    $"Botão do mouse mudou de {args.OldValue.IsPressed} para {args.NewValue.IsPressed}");
            });
        ScreenSizeChanged += OnScreenSizeChanged;
        ScreenOrientationChanged += (_, args) =>
        {
            Console.WriteLine($"Orientação mudou para {args.NewValue}");
        };
    }

    private void AddCharSizeMenuItem()
    {
        var charSizeMenuItem = new MenuItem("Char Size: " + _charSizeOptions.Current);
        charSizeMenuItem.Press += () =>
        {
            _charSizeOptions.MoveNext();
            charSizeMenuItem.Text.Text = "Char Size: " + _charSizeOptions.Current;
            _char.Diameter = _charSizeOptions.Current * ScreenInfo.Density;
        };
        MainMenu.AddItem(charSizeMenuItem);
    }

    private void AddPhysicsStateMenuItem()
    {
        var item = new MenuItem("Physics: " + _physicsOptions.Current);
        item.Press += () =>
        {
            _physicsOptions.MoveNext();
            item.Text.Text = "Physics: " + _physicsOptions.Current;
            PhysicsEngine.IsPaused = _physicsOptions.Current != PhysicsStatus.Running;
        };
        MainMenu.AddItem(item);
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