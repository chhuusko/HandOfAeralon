using UnityEngine;

public class SoftenedHide : Trait
{
    public override void ModifyIncomingDamage(ref float damage, ref float combinedModifier, Ability ability)
    {
        if (!ability || !ability.GetAbilityType().HasFlag(Ability.Type.Physical))
        {
            return;
        }

        var data = Data as DamageModifyingData;
        if (!data)
        {
            return;
        }

        var multiplier = data.DamageModifierPercent / 100f;
        combinedModifier += multiplier;
    }
}
