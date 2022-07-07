using System.Collections;

namespace SkiaGame.Helpers;

public class CircularList<T> : List<T>, IEnumerable<T>
{
    public new IEnumerator<T> GetEnumerator()
    {
        return new CircularEnumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new CircularEnumerator<T>(this);
    }
}

public class CircularEnumerator<T> : IEnumerator<T>
{
    private readonly List<T> _list;
    private int _i;

    public CircularEnumerator(List<T> list)
    {
        this._list = list;
    }

    public T Current => _list[_i];

    object IEnumerator.Current => this;

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
        _i = (_i + 1) % _list.Count;
        return true;
    }

    public void Reset()
    {
        _i = 0;
    }
}