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
        
        hpFactor *= data.DamageModifier;
    }
}
