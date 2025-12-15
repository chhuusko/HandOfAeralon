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

        Color damageColor = ColorDatabase.Instance.GetDamageColor(d.Ability);
        
        string casterName = GameTextFormatter.FactionColoredLabel(d.Caster);
        string targetName = d.Caster == d.Target ? "itself" : GameTextFormatter.FactionColoredLabel(d.Target);
        string abilityName =
            TextMarkupExtensions.Colorize(d.Ability.GetAbilityName(), ColorDatabase.Instance.AbilityColor);
        string damage = TextMarkupExtensions.Colorize(d.Damage.ToString(), damageColor);

        // Check for type of ability.
        if (d.Ability.GetAbilityType() is Ability.Type.Elemental or Ability.Type.Physical)
        {
            _text.text = $"{casterName} used {abilityName} and dealt " + 
                         $"{damage} damage to {targetName}";
        }
        else if (d.Ability.GetAbilityType() is Ability.Type.Heal)
        {
            string heal = TextMarkupExtensions.Colorize(d.Heal.ToString(), ColorDatabase.Instance.HealingColor);
            _text.text = $"{casterName} used {abilityName} and restored " + 
                         $"{heal} health to {targetName}";
        }
        else
        {
            _text.text = $"{casterName} used {abilityName} " + 
                         $"on {targetName}";
        }
    }
}
