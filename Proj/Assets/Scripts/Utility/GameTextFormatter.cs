using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class GameTextFormatter
{
    /*
     * The regex pattern here says:
     * 1) Match any word character repeated times.
     * 2) Any character except newline matched as few times as possible.
     * 3) Same as the first group but with a slash added.
     */
    private static Regex _pattern = new(@"\{(\w+)\}(.*?)\{/\1\}");

    private static string CreateTag(string text, string tag)
    {
        // Colorize the tag depending on which type of tag it is.
        switch (tag)
        {
            case "ability":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.AbilityColor);
            case "elemental_damage":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.ElementalDamageColor);
            case "physical_damage":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.PhysicalDamageColor);
            case "card_damage":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.CardDamageColor);
            case "burn":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.BurnColor);
            case "poison":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.PoisonColor);
            case "non_damaging":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.NonDamagingEffectColor);
            case "heal":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.HealingColor);
            case "health":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.HealthColor);
            case "mana":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.ManaColor);
            case "card":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.CardColor);
            case "card_keyword":
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.CardKeywordColor);
            default:
                return text;
        }
    }

    /// <summary>
    /// Replaces all tags in the description with colored labels, keeping any text between the tags the same.
    /// </summary>
    /// <param name="description">The description to check.</param>
    /// <returns>The description, with colored labels for any keyword found.</returns>
    public static string LabeledDescription(string description)
    {
        description = Regex.Replace(description, _pattern.ToString(), match =>
        {
            var tagName = match.Groups[1].Value;
            var value = match.Groups[2].Value;
            value = CreateTag(value, tagName);
            return value;
        });
        
        return description;
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
                TextMarkupExtensions.Bold(ability.GetDamage().ToString())
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
                TextMarkupExtensions.Bold(ability.GetSecondDamage().ToString())
            );
        }

        desc = LabeledDescription(desc);

        // NOTE (Calle): These just sets the color of each token in the tooltip, for now.
        //ReplaceAll(ref desc, elementalDamageToken, TextMarkupExtensions.Colorize("Elemental Damage", elementalDamageColor));
        //ReplaceAll(ref desc, physicalDamageToken, TextMarkupExtensions.Colorize("Physical Damage", physicalDamageColor));
        //ReplaceAll(ref desc, burnToken, TextMarkupExtensions.Colorize("Burn", burnColor));
        //ReplaceAll(ref desc, poisonToken, TextMarkupExtensions.Colorize("Poison", posionColor));
        //ReplaceAll(ref desc, healthToken, TextMarkupExtensions.Colorize("Health", healColor));
        //ReplaceAll(ref desc, manaToken, TextMarkupExtensions.Colorize("Mana", manaColor));
        //ReplaceAll(ref desc, cardToken, TextMarkupExtensions.Colorize("Card", cardColor));

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
    
    /// <summary>
    /// Creates a named, colored link for the character's name.
    /// </summary>
    /// <param name="character">The character to create a label for.</param>
    /// <returns>The characters name in color representing its class.</returns>
    public static string CreateCharacterNameLink(Character character)
    {
        if (character == null)
        {
            Debug.LogWarning("Character is null");
            return string.Empty;
        }
        
        Color color = ColorDatabase.Instance.GetCharacterColor(character.Data);
        string name = $"<link=\"{character.CharacterID}\"><u>{character.Data.Name}</link></u>";
        
        return TextMarkupExtensions.Colorize(name, color);
    }
    
    /// <summary>
    /// Creates a faction and class name for the character, colored according to its class.
    /// </summary>
    /// <param name="character">The character to create the label for.</param>
    /// <returns>A colored label indicating faction and character class.</returns>
    public static string FactionColoredLabel(Character character)
    {
        if (character == null)
        {
            Debug.LogWarning("Character is null");
            return string.Empty;
        }
        
        Color color = ColorDatabase.Instance.GetCharacterColor(character.Data);
        string factionName = $"{character.GetFaction().ToString()}";
        string className = $"{character.GetCharacterClass().ToString()}";
        
        return TextMarkupExtensions.Colorize(
            $"<link=\"{character.CharacterID}\"><u>{factionName} {className}</link></u>", color);
    }

    public static string CharacterColoredLabel(CharacterData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Character is null");
            return string.Empty;
        }
        
        Color color = ColorDatabase.Instance.GetCharacterColor(data);

        if (data.Faction == Faction.Friendly)
        {
            return TextMarkupExtensions.Colorize(data.Name, color);
        }
        string factionName = data.Faction.ToString();
        string className = data.CharacterClass.ToString();
        return TextMarkupExtensions.Colorize($"{factionName} {className}", color);
    }
    public static string CharacterColoredLabel(CharacterData data, string text)
    {
        if (data == null)
        {
            Debug.LogWarning("Character is null");
            return string.Empty;
        }
        Color color = ColorDatabase.Instance.GetCharacterColor(data);
        return TextMarkupExtensions.Colorize(text , color);
    }

    /// <summary>
    /// Creates a colored label for the given status effect.
    /// </summary>
    /// <param name="statusEffect">The status effect to create the label for.</param>
    /// <returns>A label for the status effect, including name and color representing its type.</returns>
    public static string StatusEffectColoredLabel(StatusEffect statusEffect)
    {
        if (statusEffect == null)
        {
            Debug.LogWarning("StatusEffect is null");
            return string.Empty;
        }
        
        string name = statusEffect.Name;
        Color color = statusEffect switch
        {
            Burn => ColorDatabase.Instance.BurnColor,
            Poison => ColorDatabase.Instance.PoisonColor,
            _ => ColorDatabase.Instance.NonDamagingEffectColor
        };

        return TextMarkupExtensions.Colorize(name, color);
    }
}
