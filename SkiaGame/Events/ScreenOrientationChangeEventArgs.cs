using SkiaGame.Info;

namespace SkiaGame.Events;

public class ScreenOrientationChangeEventArgs : EventArgs
{
    public Orientation NewValue;

    public Orientation OldValue;

    public ScreenOrientationChangeEventArgs(Orientation oldValue, Orientation newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}