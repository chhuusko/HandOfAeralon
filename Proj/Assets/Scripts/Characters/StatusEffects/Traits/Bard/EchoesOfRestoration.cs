using UnityEngine;

public class EchoesOfRestoration : Trait
{
    private float _healModifier = 1f;
    private bool _shouldReset;
    
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
    
    public override void ModifyOutgoingHeal(ref float heal, Ability ability)
    {
        if (ability is not SongOfRenewalAOE)
        {
            return;
        }
        
        if (_healModifier <= 1f)
        {
            return;
        }
        
        heal *= _healModifier;
        _shouldReset = true;
    }

    public override void OnTurnEnd()
    {
        if (_shouldReset)
        {
            _healModifier = 1f;
        }
    }
}
