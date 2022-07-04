using System.Numerics;

namespace SkiaGame.Input;

public class Mouse
{
    //Dicionário com os botões do mouse e seus valores
    private Dictionary<MouseButton, MouseInfo> MouseState { get; } = new();

    /// <summary>
    /// Indexador com informações do botão do mouse
    /// </summary>
    /// <param name="button">Botão do mouse a ser pesquisado</param>
    public MouseInfo this[MouseButton button]
    {
        get =>
            MouseState.ContainsKey(button)
                ? MouseState[button]
                : new MouseInfo(0, Vector2.Zero, false);
        internal set => MouseState[button] = value;
    }

    //No desktop, todos os cliques terão a mesma posição
    internal void UpdatePosition(Vector2 position)
    {
        foreach (var info in MouseState)
        {
            MouseState[info.Key] = new MouseInfo(info.Value.Button, position, info.Value.IsPressed);
        }
    }
}