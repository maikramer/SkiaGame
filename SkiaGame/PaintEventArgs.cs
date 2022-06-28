using SkiaSharp;

namespace SkiaGame;

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