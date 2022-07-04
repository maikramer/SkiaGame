using System.Numerics;

namespace SkiaGame.Input;

public class Mouse
{
    private Dictionary<MouseButton, MouseInfo> MouseState { get; } = new();

    public MouseInfo this[MouseButton button]
    {
        get =>
            MouseState.ContainsKey(button)
                ? MouseState[button]
                : new MouseInfo(0, Vector2.Zero, false);
        internal set => MouseState[button] = value;
    }

    public bool ContainsKey(MouseButton button) { return MouseState.ContainsKey(button); }
}