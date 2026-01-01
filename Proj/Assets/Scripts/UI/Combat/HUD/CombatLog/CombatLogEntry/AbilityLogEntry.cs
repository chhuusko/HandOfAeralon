using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityLogEntry : CombatLogEntry
{
    private static string GetTargetName(AbilityLogData d)
    {
        if (d.Caster == d.Target)
        {
            return d.Caster.GetCharacterClass() is CharacterClass.Barbarian or CharacterClass.Rogue ? "himself" :
                "herself";
        }
        else
        {
            return d.Target.GetFaction() == Faction.Friendly ?
                GameTextFormatter.ClassColoredName(d.Target) : GameTextFormatter.FactionColoredLabel(d.Target);
        }
    }
    
    public override void Initialize(CombatLogData data)
    {
        var d = (AbilityLogData)data;
        if (d == null)
        {
            return;
        }

        if (!d.Ability || !d.Target || !d.Caster)
        {
            return;
        }
        
        _image.sprite = d.Ability.GetIcon();
        
        string casterName = d.Caster.GetFaction() == Faction.Friendly ?
            GameTextFormatter.ClassColoredName(d.Caster) : GameTextFormatter.FactionColoredLabel(d.Caster);

        string targetName = GetTargetName(d);
        
        string abilityName =
            TextMarkupExtensions.Colorize(d.Ability.GetAbilityName(), ColorDatabase.Instance.AbilityColor);
        Color damageColor = ColorDatabase.Instance.GetDamageColor(d.Ability);
        string damage = TextMarkupExtensions.Colorize(d.Damage.ToString(), damageColor);
        string heal = TextMarkupExtensions.Colorize(d.Heal.ToString(), ColorDatabase.Instance.HealingColor);
        
        var abilityType = d.Ability.GetAbilityType();
        bool isElemental = abilityType.HasFlag(Ability.Type.Elemental);
        bool isPhysical = abilityType.HasFlag(Ability.Type.Physical);
        bool isHeal = abilityType.HasFlag(Ability.Type.Heal);

        // Check for type of ability.
        if (isHeal && (isPhysical || isElemental))
        {
            if (d.Damage == 0)
            {
                _text.text = $"{casterName} healed for {heal}";
            }
            else
            {
                _text.text = $"{casterName} used {abilityName} and dealt " + 
                             $"{damage} damage to {targetName}";
            }
        }
        else if (isElemental || isPhysical)
        {
            _text.text = $"{casterName} used {abilityName} and dealt " + 
                         $"{damage} damage to {targetName}";
        }
        else if (isHeal)
        {
            if (d.Damage == 0)
            {
                _text.text = $"{casterName} used {abilityName} and restored " + 
                             $"{heal} health to {targetName}";
            }
            else
            {
                damage = TextMarkupExtensions.Colorize(
                    d.Damage.ToString(), ColorDatabase.Instance.ElementalDamageColor);
                _text.text = $"{casterName} took {damage} damage";
            }
        }
        else
        {
            _text.text = $"{casterName} used {abilityName} " + 
                         $"on {targetName}";
        }
    }
}
