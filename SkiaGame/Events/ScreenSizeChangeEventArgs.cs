using SkiaSharp;

namespace SkiaGame.Events;

public class ScreenSizeChangeEventArgs : EventArgs
{
    /// <summary>
    /// Novo valor do tamanho da tela
    /// </summary>
    public readonly SKSize NewValue;
    /// <summary>
    /// Valor anterior do tamanho da tela
    /// </summary>
    public readonly SKSize OldValue;

    public ScreenSizeChangeEventArgs(SKSize oldValue, SKSize newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}