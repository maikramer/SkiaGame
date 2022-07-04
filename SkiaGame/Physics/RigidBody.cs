using System.Drawing;
using System.Numerics;
using SkiaGame.Physics.Classes;
using SkiaGame.Physics.Structs;
using SkiaSharp;

namespace SkiaGame.Physics;

public class RigidBody
{
    public enum Type
    {
        Box,
        Circle
    }

    private float _forcedMass;

    /// <summary>
    ///     Flag que diz a fisica se o corpo gera gravidade
    /// </summary>
    public bool GeneratesGravity { get; set; } = false;

    /// <summary>
    ///     Flag que diz a fisica se o corpo sofre gravidade
    /// </summary>
    public bool HasGravity { get; set; } = true;

    /// <summary>
    ///     Flag que diz à fisica se o corpo Reage a uma colisão
    /// </summary>
    public bool Locked { get; set; }

    /// <summary>
    ///     Coeficiente de Restituição do objeto, basicamente o quanto a energia é conservada ao colidir.
    /// </summary>
    public float Restitution { get; set; } = 0.95f;

    /// <summary>
    ///     Massa do Corpo, se não for setada, ela sera proporcional ao tamanho do objeto
    /// </summary>
    public float Mass
    {
        get => _forcedMass != 0 ? _forcedMass : Aabb.Area;
        set => _forcedMass = value;
    }

    /// <summary>
    ///     Tamanho do Corpo
    /// </summary>
    public SKSize Size { set => SetSize(value); get => new(Width, Height); }

    /// <summary>
    ///     Largura do corpo
    /// </summary>
    public float Width => Aabb.Max.X - Aabb.Min.X;

    /// <summary>
    ///     Altura do corpo
    /// </summary>
    public float Height => Aabb.Max.Y - Aabb.Min.Y;

    /// <summary>
    ///     Centro do Objeto
    /// </summary>
    public Vector2 Center => new(Aabb.Min.X + Width / 2, Aabb.Min.Y + Height / 2);

    /// <summary>
    ///     Posição do Objeto
    /// </summary>
    public Vector2 Position { set => SetPosition(value); get => new(Aabb.Min.X, Aabb.Min.Y); }

    /// <summary>
    ///     Perimetro utilizado no calculo de colisoes
    /// </summary>
    public AABB Aabb { get; set; }

    /// <summary>
    ///     Velocidade do corpo
    /// </summary>
    public Vector2 Velocity { get; set; }

    /// <summary>
    ///     Coeficiente de atrito do objeto
    /// </summary>
    public float Friction { get; set; } = 0.9f;

    /// <summary>
    ///     Inverso da massa do corpo
    /// </summary>
    public float MassInverse => 1 / Mass;

    /// <summary>
    ///     Formato de objeto utilizado nas colisões
    /// </summary>
    public Type ShapeType { get; set; } = Type.Box;


    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Manifold LastCollision { get; internal set; } = new();

    /// <summary>
    ///     Adiciona uma força para agir no objeto. Deve ser chamado de dentro de OnPhysicsUpdate
    /// </summary>
    /// <param name="direction">Direção da força</param>
    /// <param name="strength">Intensidade</param>
    /// <param name="timeStep">TimeStep, somente repasse</param>
    public void AddForce(Vector2 direction, float strength, float timeStep)
    {
        Velocity += direction * strength * timeStep;
    }

    public bool Contains(PointF p)
    {
        if (Aabb.Max.X > p.X && p.X > Aabb.Min.X)
        {
            if (Aabb.Max.Y > p.Y && p.Y > Aabb.Min.Y)
            {
                return true;
            }
        }

        return false;
    }

    public void Move(float dt)
    {
        if (Mass >= 1000000)
        {
            return;
        }

        RoundSpeedToZero();

        var p1 = Aabb.Min + Velocity * dt;
        var p2 = Aabb.Max + Velocity * dt;
        Aabb = new AABB { Min = p1, Max = p2 };
    }

    private void RoundSpeedToZero()
    {
        var velocity = Velocity;
        if (Math.Abs(velocity.X) + Math.Abs(velocity.Y) < .01F)
        {
            velocity.X = 0;
            velocity.Y = 0;
        }

        Velocity = velocity;
    }

    public void Move(Vector2 dVector)
    {
        if (Locked)
        {
            return;
        }

        var aabb = Aabb;
        aabb.Min += dVector;
        aabb.Max += dVector;
        Aabb = aabb;
    }

    /// <summary>
    ///     Seta a Posição. A posição é dada no canto superior esquerdo. Isso não é recomendado pois quebra a física
    /// </summary>
    /// <param name="position">Posição usando um <see cref="Vector2" /></param>
    public void SetPosition(Vector2 position)
    {
        var aabb = Aabb;
        var width = Width;
        var height = Height;
        aabb.Min.X = position.X;
        aabb.Min.Y = position.Y;
        aabb.Max.X = position.X + width;
        aabb.Max.Y = position.Y + height;
        Aabb = aabb;
    }

    /// <summary>
    ///     Seta o tamanho do dos limites do objeto.
    /// </summary>
    /// <param name="size">Tamanho utilizando <see cref="SKSize" /></param>
    public void SetSize(SKSize size)
    {
        var aabb = Aabb;
        aabb.Max.X = aabb.Min.X + size.Width;
        aabb.Max.Y = aabb.Min.Y + size.Height;
        Aabb = aabb;
    }
}