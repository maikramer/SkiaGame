using SkiaSharp;
using Topten.RichTextKit;

namespace SkiaGame.UI;

public enum TextDirection
{
    Left,
    Center,
    Right
}

public class SkiaText
{
    public string Text { get; set; }
    public SKSize Size => GetSize();

    public NotifiableStyle Style { get; } = new();
    public TextDirection TextDirection { get; set; } = TextDirection.Center;

    public SkiaText(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return Text;
    }

    private SKSize GetSize()
    {
        var rs = new RichString
        {
            DefaultStyle = Style
        };
        rs.Add(Text);
        return new SKSize(rs.MeasuredWidth, rs.MeasuredHeight);
    }

    public void Draw(SKCanvas canvas, SKPoint point)
    {
        var drawPoint = SKPoint.Empty;
        drawPoint.Y = point.Y - Size.Height / 2f;
        switch (TextDirection)
        {
            case TextDirection.Left:
                drawPoint.X = point.X - Size.Width;
                break;
            case TextDirection.Center:
                drawPoint.X = point.X - Size.Width / 2f;
                break;
            case TextDirection.Right:
                break;
            default:
                return;
        }

        var rs = new RichString()
        {
            DefaultStyle = Style
        };
        rs.Bold().Add(Text);
        rs.Paint(canvas, drawPoint, new TextPaintOptions
        {
            Edging = SKFontEdging.SubpixelAntialias
        });
    }
}