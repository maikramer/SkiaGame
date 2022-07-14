using SkiaSharp;

namespace SkiaGame.UI;

public class MenuItem
{
    private const float Tolerance = 5f;

    private SKRect _rect;

    public SKPaint BoxPaint = new()
    {
        IsAntialias = true
    };

    public MenuItem(string text)
    {
        Text = new SkiaText(text);
    }

    public SkiaText Text { get; }
    public SKSize BoxMargin { get; set; } = new(20, 20);
    public SKSize ItemSize => Text.Size + BoxMargin;
    public float CornerRadius { get; set; } = 8;

    public SKColor NormalColor { get; set; } = SKColor.Parse("#AA8A2BE2");
    public SKColor HoverColor { get; set; } = SKColor.Parse("#AA9400D3");

    public event Action? Press;

    public void SetBaseSize()
    {
    }

    public void Draw(SKCanvas canvas, SKPoint point)
    {
        var rectX = point.X - ItemSize.Width / 2f;
        var rectY = point.Y - ItemSize.Height / 2f;
        _rect = SKRect.Create(rectX, rectY, ItemSize.Width, ItemSize.Height);
        canvas.DrawRoundRect(_rect, new SKSize(CornerRadius, CornerRadius), BoxPaint);
        Text.Draw(canvas, point);
    }


    internal bool VerifyCollision(SKPoint point, bool state)
    {
        var collRect = SKRect.Create(new SKPoint(point.X - Tolerance / 2, point.Y - Tolerance / 2),
            new SKSize(Tolerance, Tolerance));
        var res = collRect.IntersectsWithInclusive(_rect);
        BoxPaint.Color = res ? HoverColor : NormalColor;
        if (res && state) Press?.Invoke();
        return res;
    }
}