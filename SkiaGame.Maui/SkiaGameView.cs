using System.Timers;
using SkiaGame.Events;
using SkiaGame.Info;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using Timer = System.Timers.Timer;

namespace SkiaGame.Maui;

public class SkiaGameView : SKCanvasView
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly BindableProperty EngineProperty = BindableProperty.Create(nameof(Engine),
        typeof(Engine), typeof(SkiaGameView), null, propertyChanged: OnEngineChanged);

    private readonly Timer _timer = new();
    private bool _initialized;
    private ScreenInfo _screenInfo = ScreenInfo.Zero;

    public SkiaGameView()
    {
    }

    // ReSharper disable once UnusedMember.Global
    public SkiaGameView(Engine engine)
    {
        Engine = engine;
    }

    public Engine? Engine
    {
        get => (Engine)GetValue(EngineProperty);
        set => SetValue(EngineProperty, value);
    }

    /// <summary>
    /// A Pessoa não deveria Reatribuir a Engine em Runtime, isso não é muito previsível e suportado.
    /// </summary>
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
            Engine.Platform.IsMaui = true;
            Engine.InternalSetScreenInfo(_screenInfo);
            Engine.InternalExecuteOnStart();
        }
    }

    private void UpdateScreenInfo(SKSize screenSize)
    {
        var width = screenSize.Width;
        var height = screenSize.Height;
        var orientation = height > width ? Orientation.Portrait : Orientation.Landscape;
        var size = new SKSize(width, height);
        var density = 1.0f;
        if (Application.Current != null)
            density = Application.Current.Windows[0].DisplayDensity;
        _screenInfo = new ScreenInfo(size, orientation, density);
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
        var screenSize = new SKSize((float)Math.Round(density * width),
            (float)Math.Round(density * height));
        UpdateScreenInfo(screenSize);
        Engine.InternalSetScreenInfo(_screenInfo);
        //OnSizeAllocated é chamado não somente quando o programa é iniciado, mas o programa cuidará disso.
        Engine.InternalExecuteOnStart();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        UpdateScreenInfo(e.Info.Size);
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