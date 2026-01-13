using UnityEngine;

public class ResonantRecovery : Trait
{
    public override void ModifyOutgoingDamage(ref float damage, ref float combinedModifier, Ability ability)
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
        
        var modifier = data.DamageModifierPercent / 100f;
        combinedModifier += modifier;
    }

    public override void OnAbilityDataCreated(AbilityExecutionData abilityData)
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
        
        int healAmount = Mathf.RoundToInt(data.HealModifierPercent * Character.GetMaxHealth() / 100f);
        Character.Heal(healAmount);
    }
}
