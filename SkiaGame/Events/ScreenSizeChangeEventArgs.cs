using SkiaSharp;

namespace SkiaGame.Events;

public class ScreenSizeChangeEventArgs : EventArgs
{
    public SKSize NewValue;

    public SKSize OldValue;

    public ScreenSizeChangeEventArgs(SKSize oldValue, SKSize newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}