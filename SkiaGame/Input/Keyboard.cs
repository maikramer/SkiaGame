namespace SkiaGame.Input;

public class Keyboard
{
    //Mantém o status de todas as teclas do teclado
    private Dictionary<KeyCode, KeyInfo> KeyboardState { get; } = new();
    /// <summary>
    /// Indexador que pesquisa informações sobre uma tecla do teclado
    /// </summary>
    /// <param name="keyCode">Tecla a ser pesquisada</param>
    public KeyInfo this[KeyCode keyCode]
    {
        get => KeyboardState.ContainsKey(keyCode) ? KeyboardState[keyCode] : new KeyInfo(false);
        internal set => KeyboardState[keyCode] = value;
    }
}