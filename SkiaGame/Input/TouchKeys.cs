using System.Numerics;
using SkiaGame.Events;
using SkiaSharp;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable MemberCanBePrivate.Global

namespace SkiaGame.Input;

public class TouchKeys
{
    private readonly List<TouchKey> _keys = new();

    private readonly SKPoint[] _setaPoints =
    {
        new(0, 0), new(-0.25f, 0.25f), new(0, 0.5f), new(0, 0.375f), new(0.25f, 0.375f),
        new(0.25f, 0.125f), new(0, 0.125f), new(0, 0)
    };

    /// <summary>
    ///     Flag que habilita ou não as teclas Touch
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Informações sobre a tecla virtual baixo
    /// </summary>
    public TouchKey Down { get; private set; } = new();

    /// <summary>
    ///     Informações sobre a tecla virtual esquerda
    /// </summary>
    public TouchKey Left { get; private set; } = new();

    /// <summary>
    ///     Informações sobre a tecla virtual direita
    /// </summary>
    public TouchKey Right { get; private set; } = new();

    /// <summary>
    ///     Informações sobre a tecla virtual cima
    /// </summary>
    public TouchKey Up { get; private set; } = new();

    /// <summary>
    ///     Obtém ou seta o tamanho base do controle touch, lembrando que ele é dependente da densidade nas plataformas móveis
    /// </summary>
    public float Size { get; set; } = 120f;

    /// <summary>
    ///     Obtém o tamanho do controle touch descontado a margem
    /// </summary>
    public float ControlSize { get; private set; }

    /// <summary>
    ///     Obtém o tamanho do botão (Ele é <see cref="ControlSize" /> / 3)
    /// </summary>
    public float ButtonSize { get; private set; }

    /// <summary>
    ///     Obtém a Margem (Ela é <see cref="Size" /> / 10)
    /// </summary>
    public float Margin { get; private set; }

    /// <summary>
    ///     A tolerancia para fora do botão em que a pessoa pode clicar
    /// </summary>
    public float TouchTolerance { get; set; } = 10;

    /// <summary>
    ///     Paint com as propriedades para desenhar o circulo do botão (Com a cor por exemplo)
    /// </summary>
    public SKPaint Paint { get; } = new()
    {
        Color = new SKColor(0, 30, 50, 30),
        IsAntialias = true,
        Style = SKPaintStyle.Fill
    };

    /// <summary>
    ///     Paint com as propriedades para desenhar as setas (Com a cor por exemplo)
    /// </summary>
    public SKPaint PaintArrows { get; } = new()
    {
        Color = new SKColor(0, 0, 0, 80),
        IsAntialias = true,
        Style = SKPaintStyle.Fill
    };

    //Utilizado internamente para redesenhar os botões
    internal void Resize(float density)
    {
        ControlSize = Size * density;
        Margin = ControlSize / 10f;
        ButtonSize = (ControlSize - Margin) / 3f;
        var baseArrow = new SKPath();
        baseArrow.AddPoly(_setaPoints);
        //Translata para que fique localizada no canto superior esquerdo
        var translate = SKMatrix.CreateTranslation(0.25f, 0);
        baseArrow.Transform(translate);
        var scaleMatrix = SKMatrix.CreateScale(ButtonSize, ButtonSize);
        baseArrow.Transform(scaleMatrix);
        Left = new TouchKey(baseArrow)
        {
            EventCode = TouchKeyEventCode.Left
        };
        var up = new SKPath(baseArrow);
        up.Transform(SKMatrix.CreateRotation((float)(Math.PI / 2), up.Bounds.MidX, up.Bounds.MidY));
        Up = new TouchKey(up)
        {
            EventCode = TouchKeyEventCode.Up
        };
        var down = new SKPath(baseArrow);
        down.Transform(SKMatrix.CreateRotation((float)(-Math.PI / 2), down.Bounds.MidX,
            down.Bounds.MidY));
        Down = new TouchKey(down)
        {
            EventCode = TouchKeyEventCode.Down
        };
        var right = new SKPath(baseArrow);
        right.Transform(SKMatrix.CreateRotation((float)Math.PI, right.Bounds.MidX,
            right.Bounds.MidY));
        Right = new TouchKey(right)
        {
            EventCode = TouchKeyEventCode.Right
        };
        _keys.AddRange(new[]
        {
            Up, Down, Left, Right
        });
    }

    public void DrawLeftBottom(SKCanvas canvas, Vector2 leftBottom)
    {
        Draw(canvas, leftBottom with
        {
            Y = leftBottom.Y - ControlSize
        });
    }

    public void DrawFromCenter(SKCanvas canvas, Vector2 center)
    {
        Draw(canvas, new Vector2(center.X - ControlSize / 2, center.Y - ControlSize / 2));
    }

    //Desenha a partir de um canto superior esquerdo padrão
    public void Draw(SKCanvas canvas, Vector2 position)
    {
        var firstOffset = ControlSize / 6;
        var secondOffset = ControlSize / 2;
        var thirdOffset = 5 * ControlSize / 6;
        var radius = ButtonSize / 2;
        var centerUpDown = position.X + secondOffset;
        var centerLeftRight = position.Y + secondOffset;
        //Cima
        //Desenha o circulo
        canvas.DrawCircle(centerUpDown, position.Y + firstOffset, radius, Paint);
        Up.Bounds = SKRect.Create(
            new SKPoint(centerUpDown - radius, position.Y + firstOffset - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(Up.Arrow, PaintArrows);
        //Baixo
        //Desenha o circulo
        canvas.DrawCircle(centerUpDown, position.Y + thirdOffset, radius, Paint);
        Down.Bounds = SKRect.Create(
            new SKPoint(centerUpDown - radius, position.Y + thirdOffset - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(Down.Arrow, PaintArrows);
        //Esquerda
        //Desenha o circulo
        canvas.DrawCircle(position.X + firstOffset, centerLeftRight, radius, Paint);
        Left.Bounds = SKRect.Create(
            new SKPoint(position.X + firstOffset - radius, centerLeftRight - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(Left.Arrow, PaintArrows);
        //Direita
        //Desenha o circulo
        canvas.DrawCircle(position.X + thirdOffset, centerLeftRight, radius, Paint);
        Right.Bounds = SKRect.Create(
            new SKPoint(position.X + thirdOffset - radius, centerLeftRight - radius),
            new SKSize(ButtonSize, ButtonSize));
        //Desenha a seta
        canvas.DrawPath(Right.Arrow, PaintArrows);
    }

    //Verifica colisão
    public TouchKeyEventCode VerifyTouchCollision(Vector2 touchPoint, bool isPress)
    {
        var touchRect = SKRect.Create(
            new SKPoint(touchPoint.X - TouchTolerance / 2, touchPoint.Y - TouchTolerance / 2),
            new SKSize(TouchTolerance, TouchTolerance));
        var lastKeyPressed = TouchKeyEventCode.None;
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var key in _keys)
        {
            key.IsPressed = isPress && touchRect.IntersectsWithInclusive(key.Bounds);
            if (key.IsPressed) lastKeyPressed = key.EventCode;
        }

        return lastKeyPressed;
    }
}