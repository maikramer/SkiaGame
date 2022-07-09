using Newtonsoft.Json;

namespace SkiaGame.UI.Settings;

public abstract class FlipSetting : IEquatable<FlipSetting>
{
    protected FlipSetting()
    {
        Status = Positive;
    }

    public string Status { get; private set; }
    [JsonIgnore] protected virtual string Positive => "Positive";
    [JsonIgnore] protected virtual string Negative => "Negative";

    public bool Equals(FlipSetting? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Status == other.Status;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((FlipSetting)obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return Status.GetHashCode();
    }

    public void Flip()
    {
        Status = Status == Positive ? Negative : Positive;
    }

    public override string ToString()
    {
        return Status;
    }
}