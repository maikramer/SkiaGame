namespace SkiaGame.Input;

public class KeyInfo
{
    /// <summary>
    /// Retorna Se o botão foi está pressionado no momento
    /// </summary>
    public readonly bool IsPressed;

    public KeyInfo(bool isPressed) { IsPressed = isPressed; }
}