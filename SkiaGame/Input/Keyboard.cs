namespace SkiaGame.Input;

public class Keyboard
{
    private Dictionary<KeyCode, KeyInfo> KeyboardState { get; } = new();

    public KeyInfo this[KeyCode keyCode]
    {
        get => KeyboardState.ContainsKey(keyCode) ? KeyboardState[keyCode] : new KeyInfo(false);
        internal set => KeyboardState[keyCode] = value;
    }

    public bool ContainsKey(KeyCode keyCode) { return KeyboardState.ContainsKey(keyCode); }
}