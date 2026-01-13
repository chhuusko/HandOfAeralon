using UnityEngine;

public class CrescendoSmash : Trait
{
    private float _totalDamageModifier;

    public override void ResetCombatState()
    {
        _totalDamageModifier = 0f;
    }

    public override void ModifyOutgoingDamage(ref float damage, ref float combinedModifier, Ability ability)
    {
        if (ability is not LuteSmash_SingleTarget)
        {
            return;
        }

        var data = Data as DamageModifyingData;
        if (!data)
        {
            return;
        }
        
        var modifier = _totalDamageModifier / 100f;
        combinedModifier += modifier;
    }

    public override void OnAbilityDataCreated(AbilityExecutionData abilityData)
    {
        if (abilityData.Ability is not LuteSmash_SingleTarget)
        {
            return;
        }

        var data = Data as DamageModifyingData;
        if (!data)
        {
            return;
        }
        
        _totalDamageModifier += data.DamageModifierPercent;
    }
}
