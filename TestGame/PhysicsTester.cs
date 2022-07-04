using System.Numerics;
using SkiaGame;
using SkiaGame.Events;
using SkiaGame.Input;
using SkiaSharp;

namespace TestGame;

public class PhysicsTester : Engine
{
    private const float GroundHeight = 30;
    private const float CharDiameter = 20;

    private readonly GameObject _char = new()
    {
        Primitive = Primitive.Circle,
        Diameter = CharDiameter,
        Color = SKColors.Crimson,
        Locked = false
    };

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
        {
            for (var j = 1; j < 14; j++)
            {
                float calcX = 0;
                while (calcX <= 0 || calcX >= ScreenInfo.Size.Width - CharDiameter)
                {
                    calcX = rand.NextSingle() * ScreenInfo.Size.Width - CharDiameter / 2;
                }

                float calcY = 0;
                while (calcY <= 0 || calcY >= ScreenInfo.Size.Height - CharDiameter)
                {
                    calcY = rand.NextSingle() * ScreenInfo.Size.Height - CharDiameter / 2;
                }

                var ball = new GameObject
                {
                    Primitive = Primitive.Circle,
                    Diameter = CharDiameter,
                    Color = SKColors.Crimson,
                    Locked = false,
                    Position = new Vector2(calcX, calcY),
                    RigidBody = { Velocity = new Vector2(rand.Next(400), rand.Next(400)) }
                };

                AddToEngine(ball);
            }
        }
    }

    protected override void OnStart()
    {
        PhysicsEngine.IsPaused = true;
        CreateAndPopulateBalls();
        _char.Position = new Vector2(100,
            ScreenInfo.Size.Height - CharDiameter - GroundHeight - 30);
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
        if (TouchKeys.Up.IsPressed || Keyboard[KeyCode.w].IsPressed ||
            Keyboard[KeyCode.W].IsPressed)
        {
            _char.RigidBody.AddForce(-Vector2.UnitY, 1000, timeStep);
        } else if (TouchKeys.Down.IsPressed || Keyboard[KeyCode.s].IsPressed ||
                   Keyboard[KeyCode.S].IsPressed)
        {
            _char.RigidBody.AddForce(Vector2.UnitY, 1000, timeStep);
        } else if (TouchKeys.Right.IsPressed || Keyboard[KeyCode.d].IsPressed ||
                   Keyboard[KeyCode.D].IsPressed)
        {
            _char.RigidBody.AddForce(Vector2.UnitX, 1000, timeStep);
        } else if (TouchKeys.Left.IsPressed || Keyboard[KeyCode.a].IsPressed ||
                   Keyboard[KeyCode.A].IsPressed)
        {
            _char.RigidBody.AddForce(-Vector2.UnitX, 1000, timeStep);
        }
    }
}