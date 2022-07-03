using GLib;
using SkiaGame;
using SkiaGame.Info;
using SkiaSharp;

namespace Gtk;

public static class Implementation
{
    [STAThread]
    public static void Run(Engine engine)
    {
        Application.Init();

        var app = new Application("com.skiagame.gtk", ApplicationFlags.None);
        app.Register(Cancellable.Current);

        var win = new MainWindow(engine);
        app.AddWindow(win);
        win.Show();
        var winWidth = win.Child.AllocatedWidth;
        var winHeight = win.Child.AllocatedHeight;
        var orientation = winHeight > winWidth
            ? SkiaGame.Info.Orientation.Portrait
            : SkiaGame.Info.Orientation.Landscape;
        var size = new SKSize(winWidth, winHeight);
        engine.InternalSetScreenInfo(new ScreenInfo(size, orientation));
        engine.InternalExecuteOnStart();
        Application.Run();
    }
}