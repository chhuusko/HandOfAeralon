using UnityEngine;

public class CrescendoSmash : Trait
{
    private float _totalDamageModifier;

    public override void ResetCombatState()
    {
        _totalDamageModifier = 0f;
    }

    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
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
        
        var modifier = 1f + _totalDamageModifier / 100f;
        damage *= modifier;
    }

    public override void OnAbilityUsed(AbilityExecutionData abilityData)
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
