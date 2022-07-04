using SkiaGame.Info;
using SkiaSharp;

namespace SkiaGame.Forms
{
    public static class Implementation
    {
        [STAThread]
        public static void Run(Engine engine)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            var win = new MainWindow(engine);
            var winWidth = win.Width - win.DockPadding.Right - win.DockPadding.Left;
            var winHeight = win.Height - win.DockPadding.Top - win.DockPadding.Bottom;
            var orientation = winHeight > winWidth
                ? Info.Orientation.Portrait
                : Info.Orientation.Landscape;
            var size = new SKSize(winWidth, winHeight);
            engine.InternalSetScreenInfo(new ScreenInfo(size, orientation));
            engine.InternalExecuteOnStart();

            Application.Run(win);
        }
    }
}