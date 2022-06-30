using System.Numerics;
using Gdk;
using SkiaGame;
using SkiaGame.Events;
using SkiaGame.Input;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using Timer = System.Timers.Timer;

namespace Gtk
{
    public class MainWindow : Window
    {
        private readonly Engine _engine;

        public MainWindow(Engine engine)
            : this(new Builder("MainWindow.glade"), engine)
        {
        }

        private MainWindow(Builder builder, Engine engine)
            : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);
            DeleteEvent += OnWindowDeleteEvent;
            ButtonPressEvent += OnButtonPressEvent;
            ButtonReleaseEvent += OnButtonReleaseEvent;
            KeyPressEvent += OnKeyPressEvent;
            var skiaView = new SKDrawingArea();
            var timer = new Timer(1000.0f / engine.FrameRate);
            timer.AutoReset = true;
            timer.Elapsed += (_, _) => { skiaView.QueueDraw(); };
            timer.Start();
            _engine = engine;
            skiaView.PaintSurface += SkiaViewOnPaintSurface;
            skiaView.Show();
            Child = skiaView;
        }

        private void OnKeyPressEvent(object o, KeyPressEventArgs args)
        {
            var keyStr = args.Event.Key.ToString();
            Enum.TryParse(keyStr, out KeyCode keyCode);
            var eventArgs = new SkKeyPressEventArgs(keyCode);
            _engine.InternalKeyPress(eventArgs);
        }

        private SkTouchEventArgs SetMouseState(EventButton eventButton, bool state)
        {
            TranslateCoordinates(Child, (int)eventButton.XRoot, (int)eventButton.YRoot,
                out var coordX, out var coordY);
            var eventArgs = new SkTouchEventArgs(new Vector2(coordX, coordY));
            var mouseInfo = new MouseInfo
            {
                Button = (MouseButton)eventButton.Button,
                ClickPosition = new Vector2(coordX, coordY),
                IsPressed = state
            };
            _engine.InternalSetMouseState(mouseInfo);
            return eventArgs;
        }

        private void OnButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            if (_engine.Mouse.MouseState.ContainsKey((MouseButton)args.Event.Button) &&
                _engine.Mouse.MouseState[(MouseButton)args.Event.Button].IsPressed) return;
            var evArgs = SetMouseState(args.Event, true);
            _engine.InternalTouchPress(evArgs);
        }

        private void OnButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            if (_engine.Mouse.MouseState.ContainsKey((MouseButton)args.Event.Button) &&
                !_engine.Mouse.MouseState[(MouseButton)args.Event.Button].IsPressed) return;
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
}