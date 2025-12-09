using UnityEngine;

public class Earthbinder : Trait
{
    public override void OnStatusEffectApplied(Character caster, StatusEffect statusEffect)
    {
        if (statusEffect is not Slowed)
        {
            return;
        }

        var data = Data as IntModifierData;

        if (!data)
        {
            return;
        }
        
        statusEffect.IncreaseDuration(data.Modifier);
    }
}
