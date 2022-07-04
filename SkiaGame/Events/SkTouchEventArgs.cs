using System.Numerics;

namespace SkiaGame.Events;

public class SkTouchEventArgs : EventArgs
{
    public readonly Vector2 Position;

    public SkTouchEventArgs(Vector2 position) { Position = position; }
}