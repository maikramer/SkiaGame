using System.Collections.ObjectModel;
using System.Numerics;

namespace SkiaGame.Input;

public class Mouse
{
    public Mouse()
    {
        var dict = new Dictionary<MouseButton, MouseBase>();
        foreach (var button in Enum.GetValues(typeof(MouseButton)))
        {
            dict[(MouseButton)button] = new MouseBase((MouseButton)button);
        }

        MouseState = new ReadOnlyDictionary<MouseButton, MouseBase>(dict);
    }

    //Dicionário com os botões do mouse e seus valores
    private ReadOnlyDictionary<MouseButton, MouseBase> MouseState { get; }

    /// <summary>
    /// Indexador com informações do botão do mouse
    /// </summary>
    /// <param name="button">Botão do mouse a ser pesquisado</param>
    public MouseBase this[MouseButton button]
    {
        get => MouseState[button];
        internal set => MouseState[button].CopyFrom(value);
    }

    public void AddListenerToButtonPress(MouseButton button, Action action)
    {
        var element = MouseState[button];
        element.Pressed += action;
    }

    public void AddListenerToButtonRelease(MouseButton button, Action action)
    {
        var element = MouseState[button];
        element.Released += action;
    }

    //No desktop, todos os cliques terão a mesma posição
    internal void UpdatePosition(Vector2 position)
    {
        foreach (var info in MouseState)
        {
            MouseState[info.Key].Position = position;
        }
    }
}