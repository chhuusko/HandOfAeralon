using UnityEngine;

public class Steelwrath : Trait
{
    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
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

        var multiplier = 1f + data.DamageModifier / 100f;
        damage *= multiplier;
    }
}
