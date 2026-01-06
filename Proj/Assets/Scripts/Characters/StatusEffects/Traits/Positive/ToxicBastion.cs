using UnityEngine;

public class ToxicBastion : Trait
{
    public override void ModifyIncomingPoisonDamage(ref float damage)
    {
        var data = Data as DamageModifyingData;
        if (!data)
        {
            return;
        }
        
        var modifier = 1f - data.DamageModifierPercent / 100f;
        damage *= modifier;
    }
}
