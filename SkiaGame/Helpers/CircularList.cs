namespace SkiaGame.Helpers;

public class CircularList<T> : List<T>
{
    private int _index;

    public CircularList(IEnumerable<T> enumerable) : base(enumerable)
    {
    }

    public CircularList(int index)
    {
        if (index < 0 || index >= Count)
            throw new Exception($"Index must between {0} and {Count}");

        _index = index;
    }

    public T Current()
    {
        return this[_index];
    }

    public T Next()
    {
        _index++;
        _index %= Count;

        return this[_index];
    }

    public bool MoveUntil(T element)
    {
        if (Count == 0) return false;
        var start = _index;
        do
        {
            Next();
            if (this[_index]!.Equals(element))
            {
                return true;
            }
        } while (!this[_index]!.Equals(element) && _index != start);

        return false;
    }

    public T Previous()
    {
        _index--;
        if (_index < 0)
            _index = Count - 1;

        return this[_index];
    }

    public void Reset()
    {
        _index = 0;
    }

    public void MoveToEnd()
    {
        _index = Count - 1;
    }
}