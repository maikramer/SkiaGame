namespace SkiaGame.Events;

public enum EventTouchKey
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
    public TouchKeyEventType EventType;
    public EventTouchKey Key;

    public TouchKeyEventArgs(EventTouchKey key, TouchKeyEventType eventType)
    {
        Key = key;
        EventType = eventType;
    }
}