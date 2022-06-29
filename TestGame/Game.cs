using SkiaGame;
using SkiaSharp;

namespace TestGame;

// public class Game : RectBouncer
public class Game : FreeFall
// public class Game : PlatformGame
{
    // ReSharper disable once InconsistentNaming
    private static readonly Game? _gameInstance = null;
    public static Game GameInstance => _gameInstance ?? new Game();

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public Engine Engine => this;
}