using UnityEngine;

public class Everflame : Trait
{
    public override void ModifyOutgoingBurnDamage(ref float damage)
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        damage += data.DamageModifier;
    }
}
