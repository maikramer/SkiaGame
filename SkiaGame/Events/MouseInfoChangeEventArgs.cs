using SkiaGame.Input;

namespace SkiaGame.Events;

public class MouseInfoChangeEventArgs : EventArgs
{
    /// <summary>
    /// Novo valor do mouse
    /// </summary>
    public readonly MouseInfo NewValue;

    /// <summary>
    /// Valor anterior do do mouse
    /// </summary>
    public readonly MouseInfo OldValue;

    public MouseInfoChangeEventArgs(MouseInfo oldValue, MouseInfo newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}