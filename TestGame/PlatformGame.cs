using System.Numerics;
using SkiaGame;
using SkiaGame.Events;
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
        Color = SKColors.Crimson
    };

    private readonly GameObject _ground = new()
    {
        Primitive = Primitive.Rect,
        ReactToCollision = false,
        HasGravity = false,
        Color = SKColors.Peru
    };

    protected override void OnStart()
    {
        _char.Position = new Vector2(10,
            ScreenSize.Height - CharDiameter - GroundHeight - 5);
        AddToEngine(_ground);
        AddToEngine(_char);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
        //Seta posição em Runtime, de acordo com que a pessoa altera o tamanho da tela
        _ground.Position = new Vector2(0, e.Info.Height - GroundHeight);
        _ground.Size = new SKSize(e.Info.Width, GroundHeight);
        if (TouchKeys.Right.IsPressed)
        {
            _char.Position += Vector2.UnitX * 3;
        }
        else if (TouchKeys.Left.IsPressed)
        {
            _char.Position -= Vector2.UnitX * 3;
        }
    }

    protected override void OnPhysicsUpdate(float timeStep)
    {
        if (TouchKeys.Up.IsPressed)
        {
            AddForce(-Vector2.UnitY, 1000, _char, timeStep);
        }
    }
}