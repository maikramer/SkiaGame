using System.Numerics;
using SkiaSharp;
using SkiaGame;


namespace TestGame;

public class PlatformGame : Engine
{
    private const float GroundHeight = 30;

    private readonly GameObject _ground = new()
    {
        Primitive = Primitive.Rect,
        ReactToCollision = false,
        HasGravity = false
    };

    protected override void OnUpdate(PaintEventArgs e, float timeStep)
    {
        //Seta posição em Runtime, de acordo que a pessoa altera o tamanho da tela
        _ground.Position = new Vector2(0, e.Info.Height - GroundHeight);
        _ground.Size = new SKSize(e.Info.Width, GroundHeight);
    }

    protected override void OnStart()
    {
        _ground.Paint.Color = SKColors.Peru;
        AddToDrawQueue(_ground);
    }
}