using UnityEngine;

public class Mightforged : Trait
{
    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        damage /= data.DamageModifier;
    }
}
