using System.Numerics;
using SkiaGame.Events;

namespace SkiaGame.Input;

public class Mouse
{
    //Dicionário com os botões do mouse e seus valores
    private class StateElement
    {
        public MouseInfo State;
        public MouseInfo LastState;

        public StateElement(MouseInfo state, MouseInfo lastState)
        {
            State = state;
            LastState = lastState;
        }

        public event Action<MouseInfoChangeEventArgs>? IsPressedChanged;

        internal void OnPressedChanged(MouseInfoChangeEventArgs obj)
        {
            IsPressedChanged?.Invoke(obj);
        }
    }

    private Dictionary<MouseButton, StateElement> MouseState { get; } = new();

    /// <summary>
    /// Indexador com informações do botão do mouse
    /// </summary>
    /// <param name="button">Botão do mouse a ser pesquisado</param>
    public MouseInfo this[MouseButton button]
    {
        get => GetOrCreate(button).State;
        internal set
        {
            if (!MouseState.ContainsKey(button))
            {
                MouseState[button] = new StateElement(MouseInfo.Invalid, MouseInfo.Invalid);
            }

            MouseState[button].LastState = MouseState[button].State;
            MouseState[button].State = value;
            if (MouseState[button].LastState.IsPressed == value.IsPressed) return;
            var args = new MouseInfoChangeEventArgs(MouseState[button].LastState,
                MouseState[button].State);
            MouseState[button].OnPressedChanged(args);
        }
    }

    private StateElement GetOrCreate(MouseButton button)
    {
        if (MouseState.ContainsKey(button)) return MouseState[button];
        var mouseInfo = new MouseInfo(button, Vector2.Zero, false);
        var element = new StateElement(mouseInfo, mouseInfo);
        MouseState[button] = element;
        return element;
    }

    public void AddListenerToButton(MouseButton button, Action<MouseInfoChangeEventArgs> action)
    {
        var element = GetOrCreate(button);
        element.IsPressedChanged += action;
    }

    //No desktop, todos os cliques terão a mesma posição
    internal void UpdatePosition(Vector2 position)
    {
        foreach (var info in MouseState)
        {
            MouseState[info.Key].LastState.Position = position;
        }
    }
}