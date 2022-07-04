using System.Numerics;
using System.Timers;
using Gdk;
using SkiaGame;
using SkiaGame.Events;
using SkiaGame.Input;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using Timer = System.Timers.Timer;

namespace Gtk;

public class MainWindow : Window
{
    private readonly Engine _engine;

    public MainWindow(Engine engine) : this(new Builder("MainWindow.glade"), engine)
    {
    }

    private MainWindow(Builder builder, Engine engine) : base(
        builder.GetObject("MainWindow").Handle)
    {
        builder.Autoconnect(this);
        DeleteEvent += OnWindowDeleteEvent;
        ButtonPressEvent += OnButtonPressEvent;
        ButtonReleaseEvent += OnButtonReleaseEvent;
        KeyPressEvent += OnKeyPressEvent;
        KeyReleaseEvent += OnKeyReleaseEvent;
        var skiaView = new SKDrawingArea();
        var timer = new Timer(1000.0f / engine.FrameRate);
        timer.AutoReset = true;
        timer.Elapsed += (_, _) => { skiaView.QueueDraw(); };
        timer.Start();

        var inputTimer = new Timer(1000.0f / engine.InputFeedRate);
        inputTimer.AutoReset = true;
        inputTimer.Elapsed += InputTimerOnElapsed;
        inputTimer.Start();

        _engine = engine;
        skiaView.PaintSurface += SkiaViewOnPaintSurface;
        skiaView.Show();
        Child = skiaView;
    }

    private void InputTimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        var devices = Display.Default.ListDevices();
        if (devices.Length == 0) return;
        devices[0].GetPosition(null, out var posX, out var posY);
        TranslateCoordinates(Child, posX, posY, out posX,
            out posY);
        _engine.InternalUpdateMouseDesktop(new Vector2(posX, posY));
    }

    private void OnKeyReleaseEvent(object o, KeyReleaseEventArgs args)
    {
        var keyStr = args.Event.Key.ToString();
        var result = Enum.TryParse(keyStr, out KeyCode keyCode);
        if (!result) return;
        var eventArgs = new SkKeyPressEventArgs(keyCode);
        _engine.InternalKeyRelease(eventArgs);
    }

    private void OnKeyPressEvent(object o, KeyPressEventArgs args)
    {
        var keyStr = args.Event.Key.ToString();
        var result = Enum.TryParse(keyStr, out KeyCode keyCode);
        if (!result) return;
        var eventArgs = new SkKeyPressEventArgs(keyCode);
        _engine.InternalKeyPress(eventArgs);
    }

    private SkTouchEventArgs SetMouseState(EventButton eventButton, bool state)
    {
        TranslateCoordinates(Child, (int)eventButton.XRoot, (int)eventButton.YRoot, out var coordX,
            out var coordY);
        var eventArgs = new SkTouchEventArgs(new Vector2(coordX, coordY));
        var mouseInfo = new MouseInfo((MouseButton)eventButton.Button, new Vector2(coordX, coordY),
            state);
        _engine.InternalSetMouseState(mouseInfo);
        return eventArgs;
    }

    private void OnButtonPressEvent(object o, ButtonPressEventArgs args)
    {
        if (_engine.Mouse[(MouseButton)args.Event.Button].IsPressed)
            return;
        var evArgs = SetMouseState(args.Event, true);
        _engine.InternalTouchPress(evArgs);
    }

    private void OnButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
    {
        if (!_engine.Mouse[(MouseButton)args.Event.Button].IsPressed)
            return;
        var evArgs = SetMouseState(args.Event, false);
        _engine.InternalTouchRelease(evArgs);
    }

    private void SkiaViewOnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var eventArgs = new PaintEventArgs(e.Info, e.Surface);
        _engine.OnPaintSurface(eventArgs);
    }

    private void OnWindowDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
    }
}