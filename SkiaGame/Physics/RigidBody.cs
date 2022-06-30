using System.Numerics;
using SkiaSharp;

namespace SkiaGame.Physics;

public class RigidBody
{
    /// <summary>
    ///     Flag que diz a fisica se o corpo reage ou não à gravidade
    /// </summary>
    public bool HasGravity { get; set; } = true;

    /// <summary>
    ///     Flag que diz à fisica se o corpo Reage a uma colisão
    /// </summary>
    public bool ReactToCollision { get; set; } = true;

    /// <summary>
    ///     Módulo de elasticidade do objeto, basicamente o quanto a energia é conservada ao colidir.
    /// </summary>
    public float Elasticity { get; set; } = 0.8f;

    /// <summary>
    ///     Velocidade atual do objeto
    /// </summary>
    public Vector2 Speed { get; set; }

    /// <summary>
    ///     Centro do Objeto
    /// </summary>
    public Vector2 Center => new(Bounds.MidX, Bounds.MidY);

    /// <summary>
    ///     Posição do Objeto
    /// </summary>
    public Vector2 Position => new(Bounds.Location.X, Bounds.Location.Y);

    /// <summary>
    ///     Perimetro do Objeto, utilizado na fisica e no desenho.
    /// </summary>
    public SKRect Bounds { get; set; }

    /// <summary>
    ///     Seta a Posição. A posição é dada no canto superior esquerdo.
    /// </summary>
    /// <param name="position">Posição usando um <see cref="Vector2" /></param>
    public void SetPosition(Vector2 position)
    {
        var temp = Bounds;
        temp.Location = new SKPoint(position.X, position.Y);
        Bounds = temp;
    }

    /// <summary>
    ///     Seta o tamanho do dos limites do objeto.
    /// </summary>
    /// <param name="size">Tamanho utilizando <see cref="SKSize" /></param>
    public void SetSize(SKSize size)
    {
        var temp = Bounds;
        temp.Size = size;
        Bounds = temp;
    }

    /// <summary>
    ///     Atualiza a posição do Objeto
    /// </summary>
    /// <param name="timeStep"></param>
    public void Update(float timeStep)
    {
        SetPosition(Position + Speed * timeStep);
    }
}