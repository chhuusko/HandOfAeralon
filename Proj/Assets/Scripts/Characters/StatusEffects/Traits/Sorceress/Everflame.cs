using UnityEngine;

public class Everflame : Trait
{
    public override void ModifyBurnDamage(ref int damage)
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        damage += (int)data.DamageModifier;
    }
}
