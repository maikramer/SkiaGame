using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace SkiaGame.Maui;

public class SkiaGameView : SKCanvasView
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly BindableProperty EngineProperty =
        BindableProperty.Create(nameof(Engine), typeof(Engine), typeof(SkiaGameView), null, propertyChanged: OnEngineChanged);

    public Engine? Engine
    {
        get => (Engine)GetValue(EngineProperty);
        set => SetValue(EngineProperty, value);
    }

    public SkiaGameView()
    {
    }

    // ReSharper disable once UnusedMember.Global
    public SkiaGameView(Engine engine)
    {
        Engine = engine;
    }

    public void Init()
    {
        if (Engine == null) return;
        var timer = new System.Timers.Timer(1000 / Engine.FrameRate);
        timer.AutoReset = true;
        timer.Elapsed += (_, _) => { InvalidateSurface(); };
        timer.Start();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        var eventArgs = new PaintEventArgs(e.Info, e.Surface);
        Engine?.OnPaintSurface(eventArgs);
    }

    private static void OnEngineChanged(BindableObject d, object oldValue, object value)
    {
        if (d is not SkiaGameView view) return;

        view.Init();
        view.InvalidateSurface();
    }
}