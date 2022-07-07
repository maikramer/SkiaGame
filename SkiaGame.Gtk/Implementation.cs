using GLib;
using Gtk.Workarounds;
using SkiaGame;
using SkiaGame.Info;
using SkiaSharp;

namespace Gtk;

public static class Implementation
{
    [STAThread]
    public static void Run(Engine engine)
    {
        var richTextWorkAround = new HarfBuzzWorkAround();
        richTextWorkAround.ApplyHarfbuzzWorkaround();
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
        engine.Platform.IsDesktop = true;
        engine.Platform.IsGtk = true;
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var dotLocal = Path.Join(home, ".local", "share");
        var gameFolder = Path.Join(dotLocal, engine.Title.Trim());
        engine.InternalSetGameFolder(gameFolder);
        engine.InternalSetScreenInfo(new ScreenInfo(size, orientation, 1.0f));
        engine.InternalExecuteOnStart();
        Application.Run();
    }
}