using UnityEngine;

public class Giantblood : Trait
{
    public override void ModifyDerivedStats(ref float hpFactor, ref float damageFactor)
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }
        
        var modifier = 1f + data.DamageModifierPercent / 100f;
        hpFactor *= modifier;
    }
}
