using SkiaGame.Helpers;

namespace SkiaGame.UI.Settings;

public class CircularSetting<T> where T : IEquatable<T>
{
    public CircularSetting(IEnumerable<T> list)
    {
        _list = new CircularList<T>(list);
    }

    private readonly CircularList<T> _list;

    public void Add(T item)
    {
        _list.Add(item);
    }

    public T this[int i]
    {
        get => _list[i];
        set => _list[i] = value;
    }

    public T Current => _list.Current();

    public void Next()
    {
        _list.Next();
    }

    public void SetValue(T value)
    {
        _list.MoveUntil(value);
    }
}