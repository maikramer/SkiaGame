using System.Collections.ObjectModel;

namespace SkiaGame.Input;

public class Keyboard
{
    public Keyboard()
    {
        var dict = new Dictionary<KeyCode, KeyBase>();
        foreach (var code in Enum.GetValues(typeof(KeyCode)))
        {
            dict[(KeyCode)code] = new KeyBase(false);
        }

        KeyboardState = new ReadOnlyDictionary<KeyCode, KeyBase>(dict);
    }

    //Mantém o status de todas as teclas do teclado
    private ReadOnlyDictionary<KeyCode, KeyBase> KeyboardState { get; }

    /// <summary>
    ///     Indexador que pesquisa informações sobre uma tecla do teclado
    /// </summary>
    /// <param name="keyCode">Tecla a ser pesquisada</param>
    public KeyBase this[KeyCode keyCode]
    {
        get => KeyboardState[keyCode];
        internal set => UpdateValues(keyCode, value);
    }

    public void UpdateValues(KeyCode keyCode, KeyBase keyBase)
    {
        KeyboardState[keyCode].IsPressed = keyBase.IsPressed;
    }
}