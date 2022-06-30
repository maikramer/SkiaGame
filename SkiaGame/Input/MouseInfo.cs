using System.Numerics;

namespace SkiaGame.Input;
public enum MouseButton
{
    LeftButton = 1,
    MiddleButton = 2,
    RightButton = 3
}
public class MouseInfo
{
    public MouseButton Button;
    public bool IsPressed;
    public Vector2 ClickPosition;
}