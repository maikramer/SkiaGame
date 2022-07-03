using SkiaSharp;

namespace SkiaGame.Info;

public enum Orientation
{
    Portrait,
    Landscape
}

public class ScreenInfo
{
    public static ScreenInfo Zero = new(SKSize.Empty, Orientation.Landscape);
    public readonly Orientation Orientation;
    public readonly SKSize Size;

    public ScreenInfo(SKSize size, Orientation orientation)
    {
        Size = size;
        Orientation = orientation;
    }

    private bool Equals(ScreenInfo other)
    {
        return Size.Equals(other.Size) && Orientation == other.Orientation;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ScreenInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Size, (int)Orientation);
    }
}