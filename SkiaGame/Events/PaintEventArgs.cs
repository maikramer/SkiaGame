using SkiaSharp;

namespace SkiaGame.Events;

public class PaintEventArgs : EventArgs
{
    public readonly SKImageInfo Info;
    public readonly SKSurface Surface;

    public PaintEventArgs(SKImageInfo info, SKSurface surface)
    {
        Info = info;
        Surface = surface;
    }
}