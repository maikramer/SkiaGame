using SkiaGame.Input;

namespace SkiaGame.Events;

public class SkKeyPressEventArgs : EventArgs
{
    public readonly KeyCode KeyCode;

    public SkKeyPressEventArgs(KeyCode windowsKeyCode) { KeyCode = windowsKeyCode; }
}