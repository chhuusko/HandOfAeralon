using UnityEngine;

public class EmpoweringAnthem : Trait
{
    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        if (abilityData.Ability is not InspiringAnthem_AOE)
        {
            return;
        }

        var data = Data as IntModifierData;

        if (!data)
        {
            return;
        }
        
        abilityData.Target?.GetStatusEffectManager()?.AddStatusEffect(new Empowered(data.Modifier));
    }
}
