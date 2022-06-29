using System.Numerics;
using SkiaSharp;
using SkiaGame;


namespace TestGame;

public class PlatformGame : Engine
{
    private const float GroundHeight = 30;
    private const float CharDiameter = 40;

    private readonly GameObject _ground = new()
    {
        Primitive = Primitive.Rect,
        ReactToCollision = false,
        HasGravity = false,
        Color = SKColors.Peru
    };

    private readonly GameObject _char = new()
    {
        Primitive = Primitive.Circle,
        Diameter = CharDiameter,
        Color = SKColors.Crimson
    };

    protected override void OnStart()
    {
        _char.Position = new Vector2(10, ScreenSize.Height - CharDiameter - GroundHeight - 5);
        AddToEngine(_ground);
        AddToEngine(_char);
    }

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
        //Seta posição em Runtime, de acordo com que a pessoa altera o tamanho da tela
        _ground.Position = new Vector2(0, e.Info.Height - GroundHeight);
        _ground.Size = new SKSize(e.Info.Width, GroundHeight);
    }
}