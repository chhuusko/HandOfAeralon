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
        Underline = 1 << 3
    }

    /// <summary>
    /// Applies all styles specified to the given text and returns it.
    /// </summary>
    /// <param name="text">The text to modify.</param>
    /// <param name="style">Flags for one or multiple styles to apply.</param>
    /// <param name="color">The color to apply, if any.</param>
    /// <returns>The text with style applied.</returns>
    public static string ApplyStyle(string text, TextStyle style, Color color = default)
    {
        // Go through each flag and apply the corresponding style if needed.
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

        if (style.HasFlag(TextStyle.Underline))
        {
            text = Underline(text);
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
    
    public static string Underline(string text)
    {
        return $"<u>{text}</u>";
    }
}
