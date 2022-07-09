#region

using System.Diagnostics;
using SkiaGame.Abstracts;
using SkiaSharp;
using Topten.RichTextKit;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

#endregion

namespace SkiaGame.UI;

/// <summary>
///     A notifiable implementation of IStyle interface that provides styling
///     information for a run of text.
/// </summary>
public class NotifiableStyle : BaseModel, IStyle, ICloneable
{
    private SKColor _backgroundColor;
    private string _fontFamily = SKTypeface.Default.FamilyName;
    private bool _fontItalic;
    private float _fontSize = 16f;
    private FontVariant _fontVariant;
    private int _fontWeight = 400;
    private SKFontStyleWidth _fontWidth;
    private float _haloBlur;
    private SKColor _haloColor;
    private float _haloWidth;
    private bool _isBold;
    private float _letterSpacing;
    private float _lineHeight = 1f;
    private bool _sealed;
    private StrikeThroughStyle _strikeThrough = StrikeThroughStyle.None;
    private SKColor _textColor = new(4278190080U);
    private Topten.RichTextKit.TextDirection _textDirection = Topten.RichTextKit.TextDirection.Auto;
    private UnderlineStyle _underlineStyle = UnderlineStyle.None;


    /// <summary>
    ///     Instantiate a NotifiableStyle
    /// </summary>
    public NotifiableStyle()
    {
        FromSkTypeFace(SKTypeface.Default);
    }

    /// <summary>
    ///     Set Bold, weight 700 or not bold, weight 400
    /// </summary>
    public bool IsBold
    {
        get => _isBold;
        set
        {
            if (CheckNotSealed()) return;
            if (SetProperty(ref _isBold, value)) FontWeight = value ? 700 : 400;
        }
    }

    #region ICloneable Members

    /// <inheritdoc />
    public object Clone()
    {
        var obj = new NotifiableStyle
        {
            _sealed = _sealed,
            _fontFamily = _fontFamily,
            _fontSize = _fontSize,
            _fontWeight = _fontWeight,
            _fontItalic = _fontItalic,
            _underlineStyle = _underlineStyle,
            _strikeThrough = _strikeThrough,
            _lineHeight = _lineHeight,
            _textColor = _textColor,
            _letterSpacing = _letterSpacing,
            _fontVariant = _fontVariant,
            _textDirection = _textDirection,
            _isBold = _isBold
        };
        return obj;
    }

    #endregion

    private bool CheckNotSealed()
    {
        if (!_sealed) return false;
        Debug.WriteLine("Style has been sealed and can't be modified");
        return true;
    }

    /// <summary>
    ///     Seals the style to prevent it from further modification
    /// </summary>
    public void Seal()
    {
        _sealed = true;
    }

    /// <summary>
    ///     Sets the font from TypeFace
    /// </summary>
    /// <param name="typeface"></param>
    public NotifiableStyle FromSkTypeFace(SKTypeface typeface)
    {
        FontFamily = typeface.FamilyName;
        FontWeight = typeface.FontWeight;
        FontItalic = typeface.IsItalic;

        return this;
    }

    /// <summary>
    ///     Modifies this style with one or more attribute changes and returns a new style
    /// </summary>
    /// <remarks>
    ///     Note this method always creates a new style instance.To avoid creating excessive
    ///     style instances, consider using the StyleManager which caches instances of styles
    ///     with the same attributes
    /// </remarks>
    /// <param name="fontFamily">The new font family</param>
    /// <param name="fontSize">The new font size</param>
    /// <param name="fontWeight">The new font weight</param>
    /// <param name="fontItalic">The new font italic</param>
    /// <param name="underline">The new underline style</param>
    /// <param name="strikeThrough">The new strike-through style</param>
    /// <param name="lineHeight">The new line height</param>
    /// <param name="textColor">The new text color</param>
    /// <param name="letterSpacing">The new letterSpacing</param>
    /// <param name="fontVariant">The new font variant</param>
    /// <param name="textDirection">The new text direction</param>
    /// <returns>A new style with the passed attributes changed</returns>
    public NotifiableStyle Modify(string fontFamily, float? fontSize = null,
        int? fontWeight = null, bool? fontItalic = null, UnderlineStyle? underline = null,
        StrikeThroughStyle? strikeThrough = null, float? lineHeight = null,
        SKColor? textColor = null, float? letterSpacing = null, FontVariant? fontVariant = null,
        Topten.RichTextKit.TextDirection? textDirection = null)
    {
        var style = new NotifiableStyle
        {
            FontFamily = fontFamily
        };
        var nullable1 = fontSize;
        style.FontSize = nullable1 ?? FontSize;
        style.FontWeight = fontWeight ?? FontWeight;
        var nullable2 = fontItalic;
        style.FontItalic = nullable2 ?? FontItalic;
        var nullable3 = underline;
        style.Underline = nullable3 ?? Underline;
        var nullable4 = strikeThrough;
        style.StrikeThrough = nullable4 ?? StrikeThrough;
        nullable1 = lineHeight;
        style.LineHeight = nullable1 ?? LineHeight;
        style.TextColor = textColor ?? TextColor;
        nullable1 = letterSpacing;
        style.LetterSpacing = nullable1 ?? LetterSpacing;
        var nullable5 = fontVariant;
        style.FontVariant = nullable5 ?? FontVariant;
        var nullable6 = textDirection;
        style.TextDirection = nullable6 ?? TextDirection;
        return style;
    }

    #region IStyle Members

    /// <summary>
    ///     The font family for text this text run (defaults to "Arial").
    /// </summary>
    public string FontFamily
    {
        get => _fontFamily;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _fontFamily, value);
        }
    }

    /// <summary>The font size for text in this run (defaults to 16).</summary>
    public float FontSize
    {
        get => _fontSize;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _fontSize, value);
        }
    }

    /// <summary>
    ///     The font weight for text in this run (defaults to 400).
    /// </summary>
    public int FontWeight
    {
        get => _fontWeight;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _fontWeight, value);
        }
    }

    public SKFontStyleWidth FontWidth
    {
        get => _fontWidth;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _fontWidth, value);
        }
    }

    /// <summary>
    ///     True if the text in this run should be displayed in an italic
    ///     font; otherwise False (defaults to false).
    /// </summary>
    public bool FontItalic
    {
        get => _fontItalic;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _fontItalic, value);
        }
    }

    /// <summary>
    ///     The underline style for text in this run (defaults to None).
    /// </summary>
    public UnderlineStyle Underline
    {
        get => _underlineStyle;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _underlineStyle, value);
        }
    }

    /// <summary>
    ///     The strike through style for the text in this run (defaults to None).
    /// </summary>
    public StrikeThroughStyle StrikeThrough
    {
        get => _strikeThrough;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _strikeThrough, value);
        }
    }

    /// <summary>
    ///     The line height for text in this run as a multiplier (defaults to 1.0).
    /// </summary>
    public float LineHeight
    {
        get => _lineHeight;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _lineHeight, value);
        }
    }

    /// <summary>
    ///     The text color for text in this run (defaults to black).
    /// </summary>
    public SKColor TextColor
    {
        get => _textColor;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _textColor, value);
        }
    }

    public SKColor BackgroundColor
    {
        get => _backgroundColor;
        set => SetProperty(ref _backgroundColor, value);
    }

    public SKColor HaloColor
    {
        get => _haloColor;
        set => SetProperty(ref _haloColor, value);
    }

    public float HaloWidth
    {
        get => _haloWidth;
        set => SetProperty(ref _haloWidth, value);
    }

    public float HaloBlur
    {
        get => _haloBlur;
        set => SetProperty(ref _haloBlur, value);
    }

    /// <summary>
    ///     The character spacing for text in this run (defaults to 0).
    /// </summary>
    public float LetterSpacing
    {
        get => _letterSpacing;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _letterSpacing, value);
        }
    }

    /// <summary>
    ///     The font variant (ie: super/sub-script) for text in this run.
    /// </summary>
    public FontVariant FontVariant
    {
        get => _fontVariant;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _fontVariant, value);
        }
    }

    /// <summary>Text direction override for this span</summary>
    public Topten.RichTextKit.TextDirection TextDirection
    {
        get => _textDirection;
        set
        {
            if (CheckNotSealed()) return;
            SetProperty(ref _textDirection, value);
        }
    }

    public char ReplacementCharacter { get; } = '\0';

    #endregion
}