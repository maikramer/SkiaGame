using SkiaGame;
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
            var skiaView = new SKDrawingArea();
            var timer = new Timer(1000 / engine.FrameRate);
            timer.AutoReset = true;
            timer.Elapsed += (_, _) => { skiaView.QueueDraw(); };
            timer.Start();
            _engine = engine;
            skiaView.PaintSurface += SkiaViewOnPaintSurface;
            skiaView.Show();
            Child = skiaView;
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