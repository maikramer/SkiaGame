using SkiaGame.Input;

namespace SkiaGame.Events;

public class SkKeyPressEventArgs : EventArgs
{
    /// <summary>
    ///     Código da tecla pressionada
    /// </summary>
    public readonly KeyCode KeyCode;

    public SkKeyPressEventArgs(KeyCode windowsKeyCode)
    {
        KeyCode = windowsKeyCode;
    }
}