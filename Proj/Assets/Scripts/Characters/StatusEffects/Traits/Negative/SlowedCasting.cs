using UnityEngine;

public class SlowedCasting : Trait
{
    private bool _effectApplied;

    public override void OnCombatStarted()
    {
        _effectApplied = false;
    }

    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        if (_effectApplied)
        {
            return;
        }

        var data = Data as IntModifierData;

        if (!data)
        {
            return;
        }
        
        _effectApplied = true;
        var ability = abilityData.Ability;
        
        ability.SetCooldown(ability.GetCooldown() + data.Modifier);
    }
}
