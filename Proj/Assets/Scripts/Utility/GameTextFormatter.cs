using UnityEngine;

public static class GameTextFormatter
{
    public static string FactionColoredLabel(Character character)
    {
        if (character == null)
        {
            Debug.LogWarning("Character is null");
            return string.Empty;
        }
        
        Color color = ColorDatabase.Instance.GetCharacterColor(character);
        string factionName = character.GetFaction().ToString();
        string className = character.GetCharacterClass().ToString();
        
        return TextMarkupExtensions.Colorize($"{factionName} {className}", color);
    }
}
