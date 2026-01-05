using UnityEngine;

public class FlameForgedSoul : Trait
{
    public override void ModifyIncomingBurnDamage(ref float damage)
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
