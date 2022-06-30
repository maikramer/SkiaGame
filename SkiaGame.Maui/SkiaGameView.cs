using System.Timers;
using SkiaGame.Events;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Timer = System.Timers.Timer;

namespace SkiaGame.Maui;

public class SkiaGameView : SKCanvasView
{
    private bool _initialized;
    private Timer _timer = new();

    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly BindableProperty EngineProperty =
        BindableProperty.Create(nameof(Engine), typeof(Engine), typeof(SkiaGameView), null, propertyChanged: OnEngineChanged);

    private SKSize _allocatedSize;

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

    public void EngineReinit()
    {
        if (Engine == null) return;
        _timer.Interval = 1000.0f / Engine.FrameRate;
        if (!_initialized)
        {
            _timer.AutoReset = true;
            _timer.Elapsed += FrameRateTimer;
            _timer.Start();
            _initialized = true;
        }
        else
        {
            Engine.InternalSetScreenSize(_allocatedSize);
            Engine.InternalExecuteOnStart();
        }
    }

    private void FrameRateTimer(object? sender, ElapsedEventArgs e)
    {
        InvalidateSurface();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (Engine == null) return;
        if (Application.Current == null) return;
        var density = Application.Current.Windows[0].DisplayDensity;
        _allocatedSize = new SKSize((float)Math.Round(density * width), (float)Math.Round(density * height));
        Engine.InternalSetScreenSize(_allocatedSize);
        Engine.InternalExecuteOnStart();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        _allocatedSize = new SKSize(e.Info.Width, e.Info.Height);
        base.OnPaintSurface(e);
        var eventArgs = new PaintEventArgs(e.Info, e.Surface);
        Engine?.OnPaintSurface(eventArgs);
    }

    private static void OnEngineChanged(BindableObject d, object oldValue, object value)
    {
        if (d is not SkiaGameView view) return;

        view.EngineReinit();
        view.InvalidateSurface();
    }
}