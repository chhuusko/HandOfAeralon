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
        
        _image.sprite = d.Ability.GetIcon();

        if (!d.Ability || !d.Target || !d.Caster)
        {
            return;
        }

        Color casterColor = ColorDatabase.Instance.GetCharacterColor(d.Caster);
        Color targetColor = ColorDatabase.Instance.GetCharacterColor(d.Target);

        // Check for type of ability.
        if (d.Ability.GetAbilityType() is Ability.Type.Elemental or Ability.Type.Physical)
        {
            _text.text = $"{(d.Target.GetFaction() == Faction.Friendly ? "friendly" : "enemy")} " +
                         $"{d.Caster.Data.ClassData.name} used {d.Ability.GetAbilityName()} and dealt " + 
                         $"{d.Damage} damage to {(d.Target.GetFaction() == Faction.Friendly ? "friendly" : "enemy")}" + 
                         $" {d.Target.Data.ClassData.name}";
        }
        else
        {
            _text.text = $"{(d.Target.GetFaction() == Faction.Friendly ? "friendly" : "enemy")} " + 
                         $"{d.Caster.Data.ClassData.name} used {d.Ability.GetAbilityName()}" + 
                         $" on{(d.Target.GetFaction() == Faction.Friendly ? " " : " enemy")} " + 
                         $"{d.Target.Data.ClassData.name}";
        }
    }
}
