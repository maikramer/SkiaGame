using SkiaGame.Events;
using SkiaSharp;

namespace SkiaGame.Input;

public class TouchKey : SkiaInputBase
{
    private readonly SKPath _noModArrow;
    private SKRect _bounds;

    public TouchKey(SKPath? arrow = null)
    {
        var path = arrow ?? new SKPath();
        _noModArrow = path;
        Arrow = path;
    }

    /// <summary>
    ///     Perímetro da tecla touch, até onde ela se estende
    /// </summary>
    public SKRect Bounds
    {
        get => _bounds;
        set
        {
            _bounds = value;
            UpdatePosition();
        }
    }

    /// <summary>
    ///     Informações sobre desenho de sua seta
    /// </summary>
    public SKPath Arrow { get; set; }

    public TouchKeyEventCode EventCode { get; init; }

    /// <summary>
    ///     Atualiza a posição da tecla
    /// </summary>
    public void UpdatePosition()
    {
        var arrowX = Bounds.MidX - Arrow.Bounds.Width / 2;
        var arrowY = Bounds.MidY - Arrow.Bounds.Height / 2;
        var newPath = new SKPath(_noModArrow);
        newPath.Transform(SKMatrix.CreateTranslation(arrowX, arrowY));
        Arrow = newPath;
    }
}