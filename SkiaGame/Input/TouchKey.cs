using SkiaSharp;

namespace SkiaGame.Input;

public class TouchKey
{
    private readonly SKPath _noModArrow;
    private SKRect _bounds;

    public SKRect Bounds
    {
        get => _bounds;
        set
        {
            _bounds = value;
            UpdatePosition();
        }
    }

    public TouchKey(SKPath arrow)
    {
        _noModArrow = arrow;
        Arrow = arrow;
    }

    public SKPath Arrow { get; set; }

    public void UpdatePosition()
    {
        var arrowX = Bounds.MidX - Arrow.Bounds.Width / 2;
        var arrowY = Bounds.MidY - Arrow.Bounds.Height / 2;
        var newPath = new SKPath(_noModArrow);
        newPath.Transform(SKMatrix.CreateTranslation(arrowX, arrowY));
        Arrow = newPath;
    }
}