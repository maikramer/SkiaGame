using SkiaGame;
using SkiaSharp;

namespace TestGame;

// public class Game : RectBouncer
public class Game : FreeFall
{
    // ReSharper disable once InconsistentNaming
    private static readonly Game? _instance = null;
    public static Game Instance => _instance ?? new Game();

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public Engine Engine => Instance;
}