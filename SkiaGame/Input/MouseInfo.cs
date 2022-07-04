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
    /// <summary>
    /// Botão do mouse
    /// </summary>
    public readonly MouseButton Button;
    /// <summary>
    /// Posição do clique
    /// </summary>
    public readonly Vector2 ClickPosition;
    /// <summary>
    /// Se está pressionado ou não
    /// </summary>
    public readonly bool IsPressed;

    public MouseInfo(MouseButton button, Vector2 clickPosition, bool isPressed)
    {
        Button = button;
        ClickPosition = clickPosition;
        IsPressed = isPressed;
    }
}