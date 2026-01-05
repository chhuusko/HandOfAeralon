using UnityEngine;

public class EchoesOfRestoration : Trait
{
    private float _healModifier;

    public override void ResetCombatState()
    {
        _healModifier = 0f;
    }

    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        if (abilityData.Ability is not ResonantBlastAOE)
        {
            return;
        }
            
        var data = Data as FloatModifierData;
        if (!data)
        {
            return;
        }
        
        var modifier = data.ModifierPercent / 100f;
        _healModifier += modifier;
    }
    
    public override void ModifyOutgoingHeal(ref float heal, Ability ability)
    {
        if (ability is not SongOfRenewalAOE)
        {
            return;
        }
        
        if (_healModifier <= 0f)
        {
            return;
        }
        
        heal *= 1f + _healModifier;
        _healModifier = 0f;
    }
}
