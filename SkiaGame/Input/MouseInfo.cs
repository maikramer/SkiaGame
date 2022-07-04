using System.Numerics;

namespace SkiaGame.Input;

public enum MouseButton
{
    Left = 1,
    Middle = 2,
    Right = 3
}

public class MouseInfo
{
    public MouseButton Button;
    public Vector2 ClickPosition;
    public bool IsPressed;

    public MouseInfo(MouseButton button, Vector2 clickPosition, bool isPressed)
    {
        Button = button;
        ClickPosition = clickPosition;
        IsPressed = isPressed;
    }
}