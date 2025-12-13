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
        
        transform.Find("Icon").GetComponent<Image>().sprite = d.ExecutionData.Ability.GetIcon();

        if (!d.ExecutionData.Ability || !d.ExecutionData.Target || !d.ExecutionData.Caster)
        {
            return;
        }

        string text;
        
        // Check for type of ability.
        if (d.ExecutionData.Ability.GetAbilityType() is Ability.Type.Elemental or Ability.Type.Physical)
        {
            text = $"{d.ExecutionData.Caster.Data.ClassData.name} used {d.ExecutionData.Ability.GetAbilityName()} and dealt " +
                   $"{d.ExecutionData.Damage} damage to{(d.ExecutionData.Target.GetFaction() == Faction.Friendly ? " " : " enemy")}" +
                   $" {d.ExecutionData.Target.Data.ClassData.name}";
        }
        else
        {
            text = $"{d.ExecutionData.Caster.Data.ClassData.name} used {d.ExecutionData.Ability.GetAbilityName()}" +
                   $" on{(d.ExecutionData.Target.GetFaction() == Faction.Friendly ? " " : " enemy")} " +
                   $"{d.ExecutionData.Target.Data.ClassData.name}";
        }
        
        transform.Find("Text").GetComponent<TMP_Text>().text = text;
    }
}
