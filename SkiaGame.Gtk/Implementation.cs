using GLib;
using SkiaGame;
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
        engine.InternalSetScreenSize(new SKSize(win.Child.AllocatedWidth,
            win.Child.AllocatedHeight));
        engine.InternalExecuteOnStart();
        Application.Run();
    }
}