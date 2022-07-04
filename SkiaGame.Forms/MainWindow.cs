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
        }

        private void MainWindowLoad(object sender, EventArgs e) { }

        private void SkiaControlOnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            var eventArgs = new PaintEventArgs(e.Info, e.Surface);
            _engine.OnPaintSurface(eventArgs);
        }

        private void skiaControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

        }
    }
}