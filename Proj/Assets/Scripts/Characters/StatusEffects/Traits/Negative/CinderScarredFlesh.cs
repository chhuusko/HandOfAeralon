using UnityEngine;

public class CinderScarredFlesh : Trait
{
    public override void ModifyIncomingBurnDamage(ref float damage)
    {
        var data = Data as DamageModifyingData;
        if (!data)
        {
            return;
        }
        
        var modifier = 1f + data.DamageModifier / 100f;
        damage *= modifier;
    }
}
