using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityLogEntry : CombatLogEntry
{
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

        Color casterColor = ColorDatabase.Instance.GetCharacterColor(d.Caster);
        Color targetColor = ColorDatabase.Instance.GetCharacterColor(d.Target);
        Color damageColor = ColorDatabase.Instance.GetDamageColor(d.Ability);
        
        string casterName = TextMarkupExtensions.Colorize(d.Caster.Data.ClassData.name, casterColor);
        string abilityName =
            TextMarkupExtensions.Colorize(d.Ability.GetAbilityName(), ColorDatabase.Instance.AbilityColor);
        string damage = TextMarkupExtensions.Colorize(d.Ability.GetDamage().ToString(), damageColor);

        string faction;
        if (d.Caster == d.Target)
        {
            faction = "itself";
        }
        else
        {
            faction = TextMarkupExtensions.Colorize(d.Target.GetFaction() + 
                $" {d.Target.GetCharacterClass()}", targetColor);
        }

        // Check for type of ability.
        if (d.Ability.GetAbilityType() is Ability.Type.Elemental or Ability.Type.Physical)
        {
            _text.text = $"{(d.Caster.GetFaction() == Faction.Friendly ? "Friendly" : "Enemy")} " +
                         $"{casterName} used {abilityName} and dealt " + 
                         $"{damage} damage to {faction}";
        }
        else
        {
            _text.text = $"{(d.Caster.GetFaction() == Faction.Friendly ? "Friendly" : "Enemy")} " + 
                         $"{casterName} used {abilityName} " + 
                         $"on {faction}";
        }
    }
}
