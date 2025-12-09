using UnityEngine;

public class BattlebornMomentum : Trait
{
    private int _abilitiesUsed;
    private bool _effectApplied;

    public override void OnCombatStarted()
    {
        _effectApplied = false;
        _abilitiesUsed = 0;
    }

    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        var data = Data as IntThresholdData;

        if (!data)
        {
            return;
        }
        
        if (++_abilitiesUsed >= data.Threshold && !_effectApplied)
        {
            _abilitiesUsed = 0;
            _effectApplied = true;
            
            Manager.AddStatusEffect(new Empowered(data.TurnAmount));
        }
    }
}
