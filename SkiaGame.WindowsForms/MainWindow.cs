using System.Numerics;
using SkiaGame.Events;
using SkiaGame.Input;
using SkiaSharp.Views.Desktop;
using PaintEventArgs = SkiaGame.Events.PaintEventArgs;

namespace SkiaGame.Forms
{
    public partial class MainWindow : Form
    {
        private readonly Engine _engine;

        public MainWindow(Engine engine)
        {
            InitializeComponent();
            var timer = new System.Timers.Timer(1000.0f / engine.FrameRate);
            timer.AutoReset = true;
            timer.Elapsed += (_, _) => { skiaControl.Invalidate(); };
            timer.Start();
            _engine = engine;
            skiaControl.PaintSurface += SkiaControlOnPaintSurface;
            skiaControl.MouseDown += SkiaControlOnMouseDown;
            skiaControl.MouseUp += SkiaControlOnMouseUp;
        }

        private void SkiaControlOnMouseDown(object? sender, MouseEventArgs e)
        {
            var res = Enum.TryParse(e.Button.ToString(), out MouseButton but);
            Console.WriteLine(res);
            if (!res) return;
            if (_engine.Mouse.MouseState.ContainsKey(but) &&
                _engine.Mouse.MouseState[but].IsPressed)
                return;
            var evArgs = SetMouseState(e, true);
            _engine.InternalTouchPress(evArgs);
        }

        private void SkiaControlOnMouseUp(object? sender, MouseEventArgs e)
        {
            var res = Enum.TryParse(e.Button.ToString(), out MouseButton but);
            Console.WriteLine(res);
            if (!res) return;
            if (_engine.Mouse.MouseState.ContainsKey(but) &&
                _engine.Mouse.MouseState[but].IsPressed)
                return;
            var evArgs = SetMouseState(e, false);
            _engine.InternalTouchRelease(evArgs);
        }

        private SkTouchEventArgs SetMouseState(MouseEventArgs eventButton, bool state)
        {
            var eventArgs = new SkTouchEventArgs(new Vector2(eventButton.X, eventButton.Y));
            var mouseInfo = new MouseInfo
            {
                Button = (MouseButton)eventButton.Button,
                ClickPosition = new Vector2(eventButton.X, eventButton.Y),
                IsPressed = state
            };
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
}