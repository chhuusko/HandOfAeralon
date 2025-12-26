using UnityEngine;

public class EchoesOfRestoration : Trait
{
    private float _healModifier = 1f;
    
    public override void ModifyOutgoingHeal(ref float heal, Ability ability)
    {
        if (_healModifier <= 1f)
        {
            return;
        }
        
        heal *= _healModifier;
        _healModifier = 1f;
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
        
        _healModifier += data.Modifier;
    }
}
