using SkiaSharp;

namespace SkiaGame.Info;

public enum Orientation
{
    Portrait,
    Landscape
}

public class ScreenInfo
{
    public static readonly ScreenInfo Zero = new(SKSize.Empty, Orientation.Landscape, 1.0f);
    public readonly Orientation Orientation;
    public readonly SKSize Size;
    public readonly float Density;

    public ScreenInfo(SKSize size, Orientation orientation, float density)
    {
        Size = size;
        Orientation = orientation;
        Density = density;
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