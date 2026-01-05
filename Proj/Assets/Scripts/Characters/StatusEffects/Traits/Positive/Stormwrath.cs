using UnityEngine;

public class Stormwrath : Trait
{
    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        if (!ability || !ability.GetAbilityType().HasFlag(Ability.Type.Elemental))
        {
            return;
        }

        var data = Data as DamageModifyingData;
        if (!data)
        {
            return;
        }

        var multiplier = 1f + data.DamageModifierPercent / 100f;
        damage *= multiplier;
    }
}
