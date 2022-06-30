namespace SkiaGame.Input;

public class Mouse
{
    public Dictionary<MouseButton, MouseInfo> MouseState { get; } = new();
}