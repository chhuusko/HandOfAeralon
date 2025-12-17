using UnityEngine;

public class Venomcraft : Trait
{
    public override bool OnTryApplyStatusEffect(Character caster, Character target, StatusEffect statusEffect)
    {
        if (target != Character)
        {
            return true;
        }
        
        if (statusEffect is not Poison)
        {
            return true;
        }

        var data = Data as IntModifierData;

        if (data == null)
        {
            return true;
        }

        statusEffect.IncreaseDuration(data.Modifier);
        
        return true;
    }
}
