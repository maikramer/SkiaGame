using SkiaGame;

namespace TestGame;

// public class Game : RectBouncer
// public class Game : FreeFall
public class Game : PreyHackGame
{
    // ReSharper disable once InconsistentNaming
    private static readonly Game? _gameInstance = null;
    public static Game GameInstance => _gameInstance ?? new Game();

    // ReSharper disable once MemberCanBeMadeStatic.Global
    public Engine Engine => this;
}