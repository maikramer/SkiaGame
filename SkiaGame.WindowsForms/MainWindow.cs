using System.Numerics;
using SkiaGame.Events;
using SkiaGame.Input;
using SkiaSharp.Views.Desktop;
using static System.Enum;
using PaintEventArgs = SkiaGame.Events.PaintEventArgs;
using Timer = System.Timers.Timer;

namespace SkiaGame.Forms;

public partial class MainWindow : Form
{
    private readonly Engine _engine;

    public MainWindow(Engine engine)
    {
        InitializeComponent();
        var timer = new Timer(1000.0f / engine.FrameRate);
        timer.AutoReset = true;
        timer.Elapsed += (_, _) => { skiaControl.Invalidate(); };
        timer.Start();
        skiaControl.MouseMove += SkiaControlOnCursorChanged;
        _engine = engine;
        skiaControl.PaintSurface += SkiaControlOnPaintSurface;
        skiaControl.MouseDown += SkiaControlOnMouseDown;
        skiaControl.MouseUp += SkiaControlOnMouseUp;
        skiaControl.KeyDown += OnKeyDownEvent;
        skiaControl.KeyUp += OnKeyUpEvent;
    }

    private void SkiaControlOnCursorChanged(object? sender, EventArgs e)
    {
        var clientPos = PointToClient(Cursor.Position);
        var position = new Vector2(clientPos.X, clientPos.Y);
        _engine.InternalUpdateMouseDesktop(position);
    }

    private void OnKeyDownEvent(object? o, KeyEventArgs keyEventArgs)
    {
        var keyStr = keyEventArgs.KeyCode.ToString();
        var result = TryParse(keyStr, out KeyCode keyCode);
        if (!result) return;
        var eventArgs = new SkKeyPressEventArgs(keyCode);
        _engine.InternalKeyPress(eventArgs);
    }

    private void OnKeyUpEvent(object? o, KeyEventArgs keyEventArgs)
    {
        var keyStr = keyEventArgs.KeyCode.ToString();
        var result = TryParse(keyStr, out KeyCode keyCode);
        if (!result) return;
        var eventArgs = new SkKeyPressEventArgs(keyCode);
        _engine.InternalKeyRelease(eventArgs);
    }

    private void SkiaControlOnMouseDown(object? sender, MouseEventArgs e)
    {
        var res = TryParse(e.Button.ToString(), out MouseButton but);
        if (!res) return;
        if (_engine.Mouse[but].IsPressed) return;
        var evArgs = SetMouseState(but, e.Location, true);
        _engine.InternalTouchPress(evArgs);
    }

    private void SkiaControlOnMouseUp(object? sender, MouseEventArgs e)
    {
        var res = TryParse(e.Button.ToString(), out MouseButton but);
        if (!res) return;
        if (!_engine.Mouse[but].IsPressed) return;
        var evArgs = SetMouseState(but, e.Location, false);
        _engine.InternalTouchRelease(evArgs);
    }

    private SkTouchEventArgs SetMouseState(MouseButton button, Point point, bool state)
    {
        var position = new Vector2(point.X, point.Y);
        var eventArgs = new SkTouchEventArgs(position);
        var mouseInfo = new MouseInfo(button, position, state);
        _engine.InternalSetMouseState(mouseInfo);
        return eventArgs;
    }

    private void MainWindowLoad(object sender, EventArgs e) { }

    private void SkiaControlOnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var eventArgs = new PaintEventArgs(e.Info, e.Surface);
        _engine.OnPaintSurface(eventArgs);
    }
}