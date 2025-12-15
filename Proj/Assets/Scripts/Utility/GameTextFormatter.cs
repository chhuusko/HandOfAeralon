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

    public static string StatusEffectColoredLabel(StatusEffect statusEffect)
    {
        if (statusEffect == null)
        {
            Debug.LogWarning("StatusEffect is null");
            return string.Empty;
        }
        
        string name = statusEffect.Name;
        Color color;
        if (statusEffect is Burn)
        {
            color = ColorDatabase.Instance.BurnColor;
        }
        else if (statusEffect is Poison)
        {
            color = ColorDatabase.Instance.PoisonColor;
        }
        else
        {
            color = ColorDatabase.Instance.NonDamagingEffectColor;
        }
        
        return TextMarkupExtensions.Colorize(name, color);
    }
}
