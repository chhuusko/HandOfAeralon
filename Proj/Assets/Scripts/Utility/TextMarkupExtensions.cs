using UnityEngine;

public static class TextMarkupExtensions
{
    [System.Flags]
    public enum TextStyle
    {
        None = 0,
        Colored = 1 << 0,
        Bold = 1 << 1,
        Italic = 1 << 2,
    }

    public static string ApplyStyle(string text, TextStyle style, Color color = default)
    {
        if (style.HasFlag(TextStyle.Colored))
        {
            text = Colorize(text, color);
        }

        if (style.HasFlag(TextStyle.Bold))
        {
            text = Bold(text);
        }

        if (style.HasFlag(TextStyle.Italic))
        {
            text = Italic(text);
        }
        
        return text;
    }
    
    public static string Colorize(string text, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
    }

    public static string Bold(string text)
    {
        return $"<b>{text}</b>";
    }

    public static string Italic(string text)
    {
        return $"<i>{text}</i>";
    }
}
