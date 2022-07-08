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

public class MouseBase : SkiaInputBase
{
    public static MouseBase Invalid { get; } = new(MouseButton.Invalid, Vector2.Zero, false);

    /// <summary>
    /// Botão do mouse
    /// </summary>
    public readonly MouseButton Button;

    /// <summary>
    /// Posição do clique
    /// </summary>
    public Vector2 Position { get; internal set; }

    public MouseBase(MouseButton button) : this(button, Vector2.Zero, false)
    {
    }

    public MouseBase(MouseButton button, Vector2 position, bool isPressed)
    {
        Button = button;
        Position = position;
        IsPressed = isPressed;
    }

    public void CopyFrom(MouseBase mouseBase)
    {
        Position = mouseBase.Position;
        IsPressed = mouseBase.IsPressed;
    }
}