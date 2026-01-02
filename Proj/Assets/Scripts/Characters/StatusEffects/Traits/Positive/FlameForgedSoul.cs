using UnityEngine;

public class FlameForgedSoul : Trait
{
    public override void ModifyBurnDamage(ref float damage)
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
