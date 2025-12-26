using UnityEngine;

public class ResonantRecovery : Trait
{
    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        var data = Data as ResonantRecoveryData;

        if (!data)
        {
            return;
        }

        if (ability is not ResonantBlastAOE)
        {
            return;
        }
        
        damage *= data.DamageModifier;
        
        
    }
}
