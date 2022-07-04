using SkiaGame.Info;

// ReSharper disable MemberCanBePrivate.Global

namespace SkiaGame.Events;

public class ScreenOrientationChangeEventArgs : EventArgs
{
    /// <summary>
    /// Novo valor da orientação
    /// </summary>
    public readonly Orientation NewValue;

    /// <summary>
    /// Valor anterior
    /// </summary>
    public readonly Orientation OldValue;

    public ScreenOrientationChangeEventArgs(Orientation oldValue, Orientation newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}