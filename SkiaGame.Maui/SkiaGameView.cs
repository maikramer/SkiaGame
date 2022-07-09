using System.Numerics;
using System.Timers;
using SkiaGame.Events;
using SkiaGame.Info;
using SkiaGame.Input;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using static System.Enum;
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
    ///     A Pessoa não deveria Reatribuir a Engine em Runtime, isso não é muito previsível e suportado.
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
        if (!EnableTouchEvents)
        {
            Console.WriteLine(
                "Os controles de Touch não funcionarão, adicione <EnableTouchEvents=True> ao seu controle SkiaGameView");
        }
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        UpdateScreenInfo(e.Info.Size);
        var eventArgs = new PaintEventArgs(e.Info, e.Surface);
        Engine?.OnPaintSurface(eventArgs);
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        base.OnTouch(e);
        var result = TryParse(e.MouseButton.ToString(), out MouseButton button);
        var location = new Vector2(e.Location.X, e.Location.Y);
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
            {
                Engine?.InternalTouchPress(
                    new SkTouchEventArgs(location));
                if (result)
                {
                    Engine?.InternalSetMouseState(new MouseBase(button, location, true));
                }

                break;
            }
            case SKTouchAction.Released:
            {
                Engine?.InternalTouchRelease(
                    new SkTouchEventArgs(location));
                if (result)
                {
                    Engine?.InternalSetMouseState(new MouseBase(button, location, false));
                }

                break;
            }
            case SKTouchAction.Moved:
            {
                if (result)
                {
                    Engine?.InternalSetMouseState(new MouseBase(button, location, true));
                }

                break;
            }
        }

        e.Handled = true;
    }

    private static void OnEngineChanged(BindableObject d, object oldValue, object value)
    {
        if (d is not SkiaGameView view) return;

        view.EngineReinit();
        view.InvalidateSurface();
    }
}