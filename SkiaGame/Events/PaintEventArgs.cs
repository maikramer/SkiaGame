using SkiaSharp;

namespace SkiaGame.Events;

public class PaintEventArgs : EventArgs
{
    /// <summary>
    /// Informação sobre a tela
    /// </summary>
    public readonly SKImageInfo Info;
    /// <summary>
    /// Informação sobre a superfície
    /// </summary>
    public readonly SKSurface Surface;

    public PaintEventArgs(SKImageInfo info, SKSurface surface)
    {
        Info = info;
        Surface = surface;
    }
}