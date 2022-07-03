using System.Numerics;

namespace SkiaGame;

public static class GameObjectEx
{
    /// <summary>
    ///     Centraliza o Objeto no centro da tela
    /// </summary>
    /// <param name="gameObject"></param>
    public static void CenterXy(this GameObject gameObject)
    {
        var engine = gameObject.Engine;
        if (engine == null)
        {
            Console.WriteLine(
                "Primeiro você deve adicionar o objeto a engine antes de usar CenterXy");
            return;
        }

        var centerScreenX = engine.ScreenInfo.Size.Width / 2.0f;
        var centerScreenY = engine.ScreenInfo.Size.Height / 2.0f;
        var halfObjX = gameObject.Size.Width / 2;
        var halfObjY = gameObject.Size.Height / 2;
        gameObject.Position = new Vector2(centerScreenX - halfObjX,
            centerScreenY - halfObjY);
    }

    /// <summary>
    ///     Centraliza o objeto no centro da tela no eixo X somente
    /// </summary>
    /// <param name="gameObject"></param>
    public static void CenterX(this GameObject gameObject)
    {
        var engine = gameObject.Engine;
        if (engine == null)
        {
            Console.WriteLine(
                "Primeiro você deve adicionar o objeto a engine antes de usar CenterX");
            return;
        }

        var centerScreen = engine.ScreenInfo.Size.Width / 2.0f;
        var halfObj = gameObject.Size.Width / 2;
        gameObject.Position = gameObject.Position with
        {
            X = centerScreen - halfObj
        };
    }

    /// <summary>
    ///     Centraliza o Objeto no centro da tela no eixo Y somente
    /// </summary>
    /// <param name="gameObject"></param>
    public static void CenterY(this GameObject gameObject)
    {
        var engine = gameObject.Engine;
        if (engine == null)
        {
            Console.WriteLine(
                "Primeiro você deve adicionar o objeto a engine antes de usar CenterY");
            return;
        }

        var centerScreen = engine.ScreenInfo.Size.Height / 2.0f;
        var halfObj = gameObject.Size.Height / 2;
        gameObject.Position = gameObject.Position with
        {
            Y = centerScreen - halfObj
        };
    }

    /// <summary>
    ///     Seta a posição em X do Objeto
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    public static void SetX(this GameObject gameObject, float x)
    {
        gameObject.Position = gameObject.Position with
        {
            X = x
        };
    }

    /// <summary>
    ///     Seta a posição em Y do Objeto
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="y"></param>
    public static void SetY(this GameObject gameObject, float y)
    {
        gameObject.Position = gameObject.Position with
        {
            Y = y
        };
    }
}