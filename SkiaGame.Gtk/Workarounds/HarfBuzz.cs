using System.Runtime.InteropServices;
using HarfBuzzSharp;
using SkiaSharp;
using Font = HarfBuzzSharp.Font;

namespace Gtk.Workarounds
{
    public class HarfBuzzWorkAround
    {
        private const int FontScale = 512;

        private FontDetails? _fontInfo;
        private SKPaint? _paint;

        internal record FontDetails(SKTypeface Typeface, Font Font);

        internal SKPaint Paint
        {
            get
            {
                var paint = _paint ??= new SKPaint
                {
                    TextEncoding = SKTextEncoding.Utf16,
                    IsStroke = false,
                    IsAntialias = true,
                    LcdRenderText = true,
                    SubpixelText = true,
                };

                paint.Typeface = FontInfo.Typeface;
                return paint;
            }
        }

        internal FontDetails FontInfo => _fontInfo ??= GetFont();

        internal float LineHeight
        {
            get
            {
                var metrics = Paint.FontMetrics;
                return metrics.Descent - metrics.Ascent;
            }
        }

        internal float AboveBaselineHeight => -Paint.FontMetrics.Ascent;

        internal float BelowBaselineHeight => Paint.FontMetrics.Descent;

        private static FontDetails GetFont()
        {
            SKTypeface skTypeFace;

            skTypeFace = SKTypeface.FromFamilyName(null, SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);

            Blob? GetTable(Face face, Tag tag)
            {
                var size = skTypeFace.GetTableSize(tag);

                if (size == 0)
                {
                    return null;
                }

                var data = Marshal.AllocCoTaskMem(size);

                var releaseDelegate = new ReleaseDelegate(() => Marshal.FreeCoTaskMem(data));

                var value = skTypeFace.TryGetTableData(tag, 0, size, data)
                    ? new Blob(data, size, MemoryMode.Writeable, releaseDelegate)
                    : null;

                return value;
            }

            var hbFace = new Face(GetTable);

            hbFace.UnitsPerEm = skTypeFace.UnitsPerEm;

            var hbFont = new Font(hbFace);
            hbFont.SetScale(FontScale, FontScale);
            hbFont.SetFunctionsOpenType();

            return new FontDetails(skTypeFace, hbFont);
        }

        /// <summary>
        /// Apply a workaround for https://github.com/mono/SkiaSharp/issues/2113
        /// </summary>
        public void ApplyHarfbuzzWorkaround()
        {
            var font = GetFont();

            using HarfBuzzSharp.Buffer buffer = new();
            buffer.ContentType = ContentType.Unicode;
            buffer.GuessSegmentProperties();

            // Force a font loading by shaping a buffer.
            font.Font.Shape(buffer);
        }
    }
}