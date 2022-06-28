using SkiaGame;

namespace Gtk
{
    public static class Implementation
    {
        [STAThread]
        public static void Run(Engine engine)
        {
            Application.Init();

            var app = new Application("com.skiagame.gtk", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow(engine);
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}