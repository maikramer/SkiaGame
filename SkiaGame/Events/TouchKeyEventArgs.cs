namespace SkiaGame.Events;

public enum TouchKey
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
    public TouchKey Key;
    public TouchKeyEventType EventType;

    public TouchKeyEventArgs(TouchKey key, TouchKeyEventType eventType)
    {
        Key = key;
        EventType = eventType;
    }
}