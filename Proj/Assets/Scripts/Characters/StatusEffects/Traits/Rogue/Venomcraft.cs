using UnityEngine;

public class Venomcraft : Trait
{
    public override void OnStatusEffectApplied(Character caster, StatusEffect statusEffect)
    {
        if (statusEffect is not Poison)
        {
            return;
        }

        var data = Data as IntModifierData;

        if (data == null)
        {
            return;
        }

        statusEffect.IncreaseDuration(data.Modifier);
    }
}
