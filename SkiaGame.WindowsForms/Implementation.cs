using SkiaGame.Info;
using SkiaSharp;
using Orientation = SkiaGame.Info.Orientation;

namespace SkiaGame.Forms;

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
        var orientation = winHeight > winWidth ? Orientation.Portrait : Orientation.Landscape;
        var size = new SKSize(winWidth, winHeight);
        engine.Platform.IsWindowsForms = true;
        engine.Platform.IsDesktop = true;
        engine.InternalSetScreenInfo(new ScreenInfo(size, orientation, 1.0f));
        engine.InternalExecuteOnStart();

        Application.Run(win);
    }
}