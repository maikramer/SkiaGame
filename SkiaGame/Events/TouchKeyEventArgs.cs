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
    public TouchKeyEventType EventType;
    public TouchKeyEventCode Key;

    public TouchKeyEventArgs(TouchKeyEventCode key, TouchKeyEventType eventType)
    {
        Key = key;
        EventType = eventType;
    }
}