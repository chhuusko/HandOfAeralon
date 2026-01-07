using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class GameTextFormatter
{
    private enum TagType
    {
        Ability,
        ElementalDamage,
        PhysicalDamage,
        CardDamage,
        Burn,
        Poison,
        NonDamagingEffect,
        Heal,
        Health,
        Mana,
        Card,
        CardKeyword
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
        
        Color color = ColorDatabase.Instance.GetCharacterColor(character);
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
        
        Color color = ColorDatabase.Instance.GetCharacterColor(character);
        string factionName = $"{character.GetFaction().ToString()}";
        string className = $"{character.GetCharacterClass().ToString()}";
        
        return TextMarkupExtensions.Colorize(
            $"<link=\"{character.CharacterID}\"><u>{factionName} {className}</link></u>", color);
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

    private static string GetTagPattern(TagType tagType)
    {
        // Find the regex pattern for the tag.
        switch (tagType)
        {
            case TagType.Ability:
                return @"{ability}(.*?){/ability}";
            case TagType.ElementalDamage:
                return @"{elemental_damage}(.*?){/elemental_damage}";
            case TagType.PhysicalDamage:
                return @"{physical_damage}(.*?){/physical_damage}";
            case TagType.CardDamage:
                return @"{card_damage}(.*?){/card_damage}";
            case TagType.Burn:
                return @"{burn}(.*?){/burn}";
            case TagType.Poison:
                return @"{poison}(.*?){/poison}";
            case TagType.NonDamagingEffect:
                return @"{non_damaging}(.*?){/non_damaging}";
            case TagType.Heal:
                return @"{heal}(.*?){/heal}";
            case TagType.Health:
                return @"{health}(.*?){/health}";
            case TagType.Mana:
                return @"{mana}(.*?){/mana}";
            case TagType.Card:
                return @"{card}(.*?){/card}";
            case TagType.CardKeyword:
                return @"{card_keyword}(.*?){/card_keyword}";
            default:
                return string.Empty;
        }
    }

    private static string CreateTag(string text, TagType tagType)
    {
        // Colorize the tag depending on which type of tag it is.
        switch (tagType)
        {
            case TagType.Ability:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.AbilityColor);
            case TagType.ElementalDamage:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.ElementalDamageColor);
            case TagType.PhysicalDamage:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.PhysicalDamageColor);
            case TagType.CardDamage:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.CardDamageColor);
            case TagType.Burn:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.BurnColor);
            case TagType.Poison:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.PoisonColor);
            case TagType.NonDamagingEffect:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.NonDamagingEffectColor);
            case TagType.Heal:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.HealingColor);
            case TagType.Health:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.HealthColor);
            case TagType.Mana:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.ManaColor);
            case TagType.Card:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.CardColor);
            case TagType.CardKeyword:
                return TextMarkupExtensions.Colorize(text, ColorDatabase.Instance.CardKeywordColor);
            default:
                return string.Empty;
        }
    }

    /// <summary>
    /// Replaces the text in the given string with a colored tag.
    /// </summary>
    /// <param name="text">Reference to the text to replace.</param>
    /// <param name="tagType">The tags to replace.</param>
    private static void ReplaceText(ref string text, TagType tagType)
    {
        var pattern = GetTagPattern(tagType);
        text = Regex.Replace(text, pattern, match =>
        {
            var value = match.Groups[1].Value;
            value = CreateTag(value, tagType);
            return value;
        });
    }
    
    /// <summary>
    /// Replaces all tags in the status effects description with colored labels, keeping any text between the tags the same.
    /// </summary>
    /// <param name="statusEffect">The status effect description to search.</param>
    /// <returns>The status effect description, with colored labels for any keyword.</returns>
    public static string LabeledStatusEffectDescription(StatusEffect statusEffect)
    {
        var description = statusEffect.Data.Description;

        foreach (var value in Enum.GetValues(typeof(TagType)))
        {
            ReplaceText(ref description, (TagType)value);
        }
        
        return description;
    }

    /// <summary>
    /// Replaces all tags in the description with colored labels, keeping any text between the tags the same.
    /// </summary>
    /// <param name="description">The description to check.</param>
    /// <returns>The description, with colored labels for any keyword found.</returns>
    public static string LabeledDescription(string description)
    {
        foreach (var value in Enum.GetValues(typeof(TagType)))
        {
            ReplaceText(ref description, (TagType)value);
        }
        
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
        ReplaceAll(ref desc, elementalDamageToken, TextMarkupExtensions.Colorize("Elemental Damage", elementalDamageColor));
        ReplaceAll(ref desc, physicalDamageToken, TextMarkupExtensions.Colorize("Physical Damage", physicalDamageColor));
        ReplaceAll(ref desc, burnToken, TextMarkupExtensions.Colorize("Burn", burnColor));
        ReplaceAll(ref desc, poisonToken, TextMarkupExtensions.Colorize("Poison", posionColor));
        ReplaceAll(ref desc, healthToken, TextMarkupExtensions.Colorize("Health", healColor));
        ReplaceAll(ref desc, manaToken, TextMarkupExtensions.Colorize("Mana", manaColor));
        ReplaceAll(ref desc, cardToken, TextMarkupExtensions.Colorize("Card", cardColor));

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
