using System.Numerics;

namespace SkiaGame.Events;

public class SkTouchEventArgs : EventArgs
{
    /// <summary>
    /// Posição em que o mouse ou touch ocorreu
    /// </summary>
    public readonly Vector2 Position;

    public SkTouchEventArgs(Vector2 position) { Position = position; }
}