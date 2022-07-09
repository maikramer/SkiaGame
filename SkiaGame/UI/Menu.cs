using SkiaGame.Events;
using SkiaSharp;

namespace SkiaGame.UI;

public class Menu
{
    public readonly List<MenuItem> Items = new();
    private float _density = 1.0f;
    public bool Enabled { get; set; } = true;
    public float ItemSpacing { get; set; } = 5f;

    public float BaseFontSize { get; set; } = 20f;

    public MenuItem this[int i] => Items[i];

    public void AddItem(MenuItem item)
    {
        item.Text.Style.FontSize = BaseFontSize * _density;
        Items.Add(item);
    }

    public void UpdateSizes(float density)
    {
        _density = density;
        foreach (var item in Items)
        {
            //Corrige a densidade
            item.Text.Style.FontSize = BaseFontSize * density;
        }
    }

    public void VerifyClick(SKPoint point, bool state)
    {
        foreach (var menuItem in Items)
        {
            menuItem.VerifyCollision(point, state);
        }
    }

    public void Draw(PaintEventArgs e)
    {
        var surface = e.Surface;
        var size = e.Info.Size;
        var canvas = surface.Canvas;
        var totalSize = new SKSize(0, 0);
        var spacing = new SKSize(0, ItemSpacing);
        foreach (var item in Items)
        {
            totalSize += item.ItemSize + spacing;
        }

        //Tira 1 pois é (n-1)
        totalSize -= spacing;

        var screenCenter = new SKPoint(size.Width / 2f, size.Height / 2f);
        var itemY = screenCenter.Y - totalSize.Height / 2f;
        foreach (var item in Items)
        {
            item.Draw(canvas, new SKPoint(screenCenter.X, itemY));
            itemY += item.ItemSize.Height + ItemSpacing;
        }
    }
}