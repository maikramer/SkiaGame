using System.Numerics;
using SkiaGame;
using SkiaGame.Events;
using SkiaGame.Physics;
using SkiaSharp;

namespace TestGame;

public class PhysicsTester : Engine
{
    private const float GroundHeight = 30;
    private const float CharDiameter = 20;
    private bool _notStarted = true;
    private readonly TimeSpan _startTime = TimeSpan.FromSeconds(6);

    private readonly GameObject _char = new()
    {
        Primitive = Primitive.Circle,
        Diameter = CharDiameter,
        Color = SKColors.Crimson,
        Locked = false
    };

    private readonly GameObject _ground = new()
    {
        Primitive = Primitive.Rect,
        Locked = true,
        Color = SKColors.Peru
    };

    private void CreateAndPopulateBalls()
    {
        var rand = new Random(DateTime.Now.Millisecond);
        for (var i = 0; i < 16; i++)
        {
            for (var j = 0; j < 12; j++)
            {
                var ball = new GameObject
                {
                    Primitive = Primitive.Circle,
                    Diameter = CharDiameter,
                    Color = SKColors.Crimson,
                    Locked = false,
                    Position = new Vector2(i * 40 + rand.Next(50),
                        ScreenSize.Height - CharDiameter - GroundHeight - j * 50 + rand.Next(50)),
                    RigidBody =
                    {
                        ShapeType = RigidBody.Type.Circle,
                        Velocity = new Vector2(rand.Next(300), rand.Next(300))
                    }
                };

                AddToEngine(ball);
            }
        }
    }

    protected override void OnStart()
    {
        PhysicsEngine.IsPaused = true;
        CreateAndPopulateBalls();
        _char.RigidBody.ShapeType = RigidBody.Type.Circle;
        _char.Position = new Vector2(100,
            ScreenSize.Height - CharDiameter - GroundHeight - 30);
        AddToEngine(_char);
        PhysicsEngine.CreateBoundingBox(ScreenSize);
        ScreenSizeChanged += OnScreenSizeChanged;
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
        if (TouchKeys.Up.IsPressed)
        {
            _char.RigidBody.AddForce(-Vector2.UnitY, 1000, timeStep);
        }
        else if (TouchKeys.Down.IsPressed)
        {
            _char.RigidBody.AddForce(Vector2.UnitY, 1000, timeStep);
        }
        else if (TouchKeys.Right.IsPressed)
        {
            _char.RigidBody.AddForce(Vector2.UnitX, 1000, timeStep);
        }
        else if (TouchKeys.Left.IsPressed)
        {
            _char.RigidBody.AddForce(-Vector2.UnitX, 1000, timeStep);
        }
    }
}