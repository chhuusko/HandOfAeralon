using UnityEngine;

public class Ironhide : Trait
{
    public override void ModifyIncomingDamage(ref float damage, Ability ability)
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

        var multiplier = 1f - data.DamageModifierPercent / 100f;
        damage *= multiplier;
    }
}
