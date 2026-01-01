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
    
    [Header("Status Effects")]
    public Color NonDamagingEffectColor;
    public Color BurnColor;
    public Color PoisonColor;

    [Header("Cards")] 
    public Color CardColor;
    public Color ManaColor;
    
    [Header("Misc")]
    public Color HealingColor;
    
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

    public Color GetDamageColor(Ability ability)
    {
        if (ability == null)
        {
            Debug.LogError($"{ability} is null");
            return Color.white;
        }
        var type = ability.GetAbilityType();

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
