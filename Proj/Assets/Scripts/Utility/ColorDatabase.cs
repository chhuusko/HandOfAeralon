using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Color Database", menuName = "UI/Color Database")]
public class ColorDatabase : ScriptableObject
{
    private static ColorDatabase instance;

    public static ColorDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<ColorDatabase>("Color Database");
            }
            return instance;
        }
    }
    
    [Header("Characters")]
    public Color BarbarianColor;
    public Color BardColor;
    public Color RogueColor;
    public Color SorceressColor;
    public Color EnemyColor;

    [Header("Abilities")] 
    public Color AbilityColor;
    public Color ElementalDamageColor;
    public Color PhysicalDamageColor;
    public Color CardDamageColor;
    
    [Header("Status Effects")]
    public Color NonDamagingEffectColor;
    public Color BurnColor;
    public Color PoisonColor;

    [Header("Cards")] 
    public Color CardColor;
    public Color ManaColor;
    public Color CardKeywordColor;
    
    [Header("Misc")]
    public Color HealingColor;
    public Color TooltipTextColor;
    
    /// <summary>
    /// Gets the UI color representing a character.
    /// </summary>
    /// <param name="c">The character to get a color for.</param>
    /// <returns>The characters corresponding color.</returns>
    public Color GetCharacterColor(Character c)
    {
        if (c == null)
        {
            Debug.LogError($"{c} is null");
            return Color.white;
        }

        if (c.GetFaction() == Faction.Enemy)
        {
            return EnemyColor;
        }

        return c.GetCharacterClass() switch
        {
            CharacterClass.Barbarian => BarbarianColor,
            CharacterClass.Bard => BardColor,
            CharacterClass.Rogue => RogueColor,
            CharacterClass.Sorceress => SorceressColor,
            _ => Color.white
        };
    }

    /// <summary>
    /// Returns a color for the given ability type.
    /// </summary>
    /// <param name="ability"></param>
    /// <returns>The color representing the given ability.</returns>
    public Color GetAbilityColor(Ability ability)
    {
        if (ability == null)
        {
            Debug.LogError($"{ability} is null");
            return Color.white;
        }
        var type = ability.GetAbilityType();

        /*
         * Check ability flags and return the color. Goes in order, so if an ability has multiple types, it will 
         * always return the first type found.
         */
        if (type.HasFlag(Ability.Type.Elemental))
        {
            return ElementalDamageColor;
        }
        if (type.HasFlag(Ability.Type.Physical))
        {
            return PhysicalDamageColor;
        }
        return type.HasFlag(Ability.Type.Heal) ? HealingColor : NonDamagingEffectColor;
    }
}
