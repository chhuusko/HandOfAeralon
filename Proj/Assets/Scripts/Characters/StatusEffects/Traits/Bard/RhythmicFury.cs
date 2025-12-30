using UnityEngine;

public class RhythmicFury : Trait
{
    private float _totalDamageIncrease = 1;

    public override void ResetCombatState()
    {
        _totalDamageIncrease = 1;
    }

    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        var abilityType = abilityData.Ability.GetAbilityType();
        if (abilityType is Ability.Type.Elemental or Ability.Type.Physical)
        {
            return;
        }
        
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        _totalDamageIncrease += data.DamageModifier;
    }
    
    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        if (ability is not LuteSmash_SingleTarget)
        {
            return;
        }
        
        damage *= _totalDamageIncrease;
    }
}
