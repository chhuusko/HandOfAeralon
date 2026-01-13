using UnityEngine;

public class BattlebornMomentum : Trait
{
    private int _abilitiesUsed;
    private bool _effectApplied;

    public override void ResetCombatState()
    {
        _abilitiesUsed = 0;
        _effectApplied = false;
    }

    // public override void OnAbilityDataCreated(AbilityExecutionData abilityData)
    // {
    //     var data = Data as IntThresholdData;
    //     if (!data)
    //     {
    //         return;
    //     }
    //
    //     if (++_abilitiesUsed < data.Threshold || _effectApplied) return;
    //     _abilitiesUsed = 0;
    //     _effectApplied = true;
    //         
    //     Manager.AddStatusEffect(new Empowered(data.TurnAmount));
    // }

    public override void OnAbilityCast(Character character, Ability ability)
    {
        var data = Data as IntThresholdData;
        if (!data)
        {
            return;
        }
        
        Debug.Log("Ability Cast");

        if (++_abilitiesUsed < data.Threshold || _effectApplied) return;
        _abilitiesUsed = 0;
        _effectApplied = true;
        Manager.AddStatusEffect(new Empowered(data.TurnAmount));
    }
}
