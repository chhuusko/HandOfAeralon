using UnityEngine;

public class ResonantRecovery : Trait
{
    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        if (ability is not ResonantBlastAOE)
        {
            return;
        }
        
        var data = Data as ResonantRecoveryData;

        if (!data)
        {
            return;
        }
        
        var modifier = 1f + data.DamageModifierPercent / 100f;
        damage *= modifier;
    }

    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        if (abilityData.Ability is not ResonantBlastAOE)
        {
            return;
        }
        
        var data = Data as ResonantRecoveryData;
        if (!data)
        {
            return;
        }
        
        int healAmount = Mathf.RoundToInt(data.HealModifier * Character.GetMaxHealth());
        Character.Heal(healAmount);
    }
}
