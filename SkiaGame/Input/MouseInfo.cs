using System.Numerics;

namespace SkiaGame.Input;

public enum MouseButton
{
    Left = 1,
    LeftButton = 1,
    Middle = 2,
    MiddleButton = 2,
    Right = 3,
    RightButton = 3,
    Invalid = int.MaxValue
}

public class MouseInfo
{
    public static MouseInfo Invalid { get; } = new(MouseButton.Invalid, Vector2.Zero, false);

    /// <summary>
    /// Botão do mouse
    /// </summary>
    public readonly MouseButton Button;

    /// <summary>
    /// Posição do clique
    /// </summary>
    public Vector2 Position { get; internal set; }

    /// <summary>
    /// Se está pressionado ou não
    /// </summary>
    public readonly bool IsPressed;

    public MouseInfo(MouseButton button, Vector2 position, bool isPressed)
    {
        Button = button;
        Position = position;
        IsPressed = isPressed;
    }
}