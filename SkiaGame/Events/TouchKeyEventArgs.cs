namespace SkiaGame.Events;

public enum TouchKeyEventCode
{
    None,
    Left,
    Right,
    Up,
    Down
}

public enum TouchKeyEventType
{
    Press,
    Release
}

public class TouchKeyEventArgs : EventArgs
{
    /// <summary>
    /// Tipo de evento de Touch
    /// </summary>
    public readonly TouchKeyEventType EventType;
    /// <summary>
    /// Qual a tecla de Touch foi pressionada
    /// </summary>
    public readonly TouchKeyEventCode Key;

    public TouchKeyEventArgs(TouchKeyEventCode key, TouchKeyEventType eventType)
    {
        Key = key;
        EventType = eventType;
    }
}