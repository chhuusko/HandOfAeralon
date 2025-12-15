using UnityEngine;

public static class TextMarkupExtensions
{
    public static string Colorize(string text, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
    }
}
