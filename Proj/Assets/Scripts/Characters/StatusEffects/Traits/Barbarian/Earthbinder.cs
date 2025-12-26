using UnityEngine;

public class Earthbinder : Trait
{
    public override bool BeforeStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        if (target != Character)
        {
            return true;
        }
        
        if (statusEffect is not Slowed or Weakened)
        {
            return true;
        }

        var data = Data as IntModifierData;

        if (!data)
        {
            return true;
        }
        
        statusEffect.IncreaseDuration(data.Modifier);
        
        return true;
    }
}
