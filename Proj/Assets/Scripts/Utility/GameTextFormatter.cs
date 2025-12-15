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

    public static string AbilityColoredLabel(Ability ability)
    {
        var colorDB = ColorDatabase.Instance;
        Color abilityColor         = colorDB.AbilityColor;
        Color elementalDamageColor = colorDB.ElementalDamageColor;
        Color physicalDamageColor  = colorDB.PhysicalDamageColor;
        Color nonDamageEffectColor = colorDB.NonDamagingEffectColor;
        Color burnColor            = colorDB.BurnColor;
        Color posionColor          = colorDB.PoisonColor;
        Color cardColor            = colorDB.CardColor;
        Color manaColor            = colorDB.ManaColor;
        Color healColor            = colorDB.HealingColor;
        

        string desc                 = ability.GetDescription();
        string damageToken          = "{damage}";
        string elementalDamageToken = "{elemental_damage}";
        string physicalDamageToken  = "{physical_damage}";
        string burnToken            = "{burn}";
        string poisonToken          = "{poison}";
        string healthToken          = "{health}";
        string manaToken            = "{mana}";
        string cardToken            = "{card}";

        // NOTE (Calle): Damages are different and capped to 2 cheks since they have only 2 functions that determine the tooltip text.
        // First occurrence
        int first = desc.IndexOf(damageToken);
        if (first != -1)
        {
            desc = ReplaceAt(
                desc,
                first,
                damageToken.Length,
                TextMarkupExtensions.Colorize(ability.GetDamage().ToString(), physicalDamageColor)
            );
        }

        // Second occurrence
        int second = desc.IndexOf(damageToken);
        if (second != -1)
        {
            desc = ReplaceAt(
                desc,
                second,
                damageToken.Length,
                TextMarkupExtensions.Colorize(ability.GetSecondDamage().ToString(), physicalDamageColor)
            );
        }

        // NOTE (Calle): These just sets the color of each token in the tooltip, for now.
        ReplaceAll(ref desc, elementalDamageToken, TextMarkupExtensions.Colorize(" Elemental Damage ", elementalDamageColor));
        ReplaceAll(ref desc, physicalDamageToken, TextMarkupExtensions.Colorize(" Physical Damage ", physicalDamageColor));
        ReplaceAll(ref desc, burnToken, TextMarkupExtensions.Colorize(" Burn ", burnColor));
        ReplaceAll(ref desc, poisonToken, TextMarkupExtensions.Colorize(" Poison ", posionColor));
        ReplaceAll(ref desc, healthToken, TextMarkupExtensions.Colorize(" Health ", healColor));
        ReplaceAll(ref desc, manaToken, TextMarkupExtensions.Colorize(" Mana ", manaColor));
        ReplaceAll(ref desc, cardToken, TextMarkupExtensions.Colorize(" Card ", cardColor));

        return desc;
    }
    private static string ReplaceAt(string text, int index, int length, string replacement)
    {
        return text.Substring(0, index) +
               replacement +
               text.Substring(index + length);
    }

    private static void ReplaceAll( ref string text, string token, string replacement)
    {
        int index;
        while ((index = text.IndexOf(token)) != -1)
        {
            text = ReplaceAt(text, index, token.Length, replacement);
        }
    }
}
