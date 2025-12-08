using UnityEngine;

public class StormlitEmberwake : Trait
{
    public override void ModifyStunApplicationChance(ref float chance)
    {
        if (!Manager.ContainsStatusEffect<Emberwake>())
        {
            return;
        }
        
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        chance *= data.DamageModifier;
    }
}
