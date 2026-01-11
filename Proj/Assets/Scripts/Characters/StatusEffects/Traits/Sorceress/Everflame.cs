using UnityEngine;

public class Everflame : Trait
{
    public override void ModifyOutgoingBurnDamage(ref float damage)
    {
        var data = Data as IntModifierData;
        if (!data)
        {
            return;
        }

        damage += data.Modifier;
    }
}
