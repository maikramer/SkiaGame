namespace SkiaGame;

public abstract class Engine
{
    public double FrameRate { get; set; } = 60;
    private DateTime _lastTime = DateTime.Now;

    public void OnPaintSurface(PaintEventArgs e)
    {
        var timeStep = (float)((DateTime.Now - _lastTime).TotalMilliseconds) / 1000.0f;
        OnUpdate(e, timeStep);
        _lastTime = DateTime.Now;
    }

    protected abstract void OnUpdate(PaintEventArgs e, float timeStep);
}