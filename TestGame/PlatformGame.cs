using System.Numerics;
using SkiaGame;
using SkiaGame.Events;
using SkiaGame.Physics;
using SkiaSharp;

namespace TestGame;

public class PlatformGame : Engine
{
    private const float GroundHeight = 30;
    private const float CharDiameter = 40;

    private readonly GameObject _char = new()
    {
        Primitive = Primitive.Circle,
        Diameter = CharDiameter,
        Color = SKColors.Crimson,
        Locked = false,
    };

    private readonly GameObject _char2 = new()
    {
        Primitive = Primitive.Circle,
        Diameter = CharDiameter,
        Color = SKColors.Crimson,
        Locked = false,
    };

    private readonly GameObject _char3 = new()
    {
        Primitive = Primitive.Circle,
        Diameter = CharDiameter,
        Color = SKColors.Crimson,
        Locked = false,
    };

    private readonly GameObject _ground = new()
    {
        Primitive = Primitive.Rect,
        Locked = true,
        Color = SKColors.Peru
    };

    protected override void OnStart()
    {
        _char.Position = new Vector2(100,
            ScreenSize.Height - CharDiameter - GroundHeight - 30);
        _char.RigidBody.ShapeType = RigidBody.Type.Circle;
        _char2.Position = new Vector2(110,
            ScreenSize.Height - CharDiameter - GroundHeight - 90);
        _char2.RigidBody.ShapeType = RigidBody.Type.Circle;
        _char3.Position = new Vector2(120,
            ScreenSize.Height - CharDiameter - GroundHeight - 120);
        _char3.RigidBody.ShapeType = RigidBody.Type.Circle;


        AddToEngine(_ground);
        AddToEngine(_char);
        AddToEngine(_char2);
        AddToEngine(_char3);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
        //Seta posição em Runtime, de acordo com que a pessoa altera o tamanho da tela
        _ground.Position = new Vector2(0, e.Info.Height - GroundHeight);
        _ground.Size = new SKSize(e.Info.Width, GroundHeight);
    }

    protected override void OnPhysicsUpdate(float timeStep)
    {
        if (TouchKeys.Up.IsPressed)
        {
            AddForce(-Vector2.UnitY, 1000, _char, timeStep);
        }
        else if (TouchKeys.Right.IsPressed)
        {
            _char.Position += Vector2.UnitX * 3;
        }
        else if (TouchKeys.Left.IsPressed)
        {
            _char.Position -= Vector2.UnitX * 3;
        }
    }
}