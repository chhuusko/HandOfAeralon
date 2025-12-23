using UnityEngine;

public class HealingFatigue : StatusEffect
{
    public override void ModifyIncomingHeal(ref float heal, Ability ability)
    {
        // Only affects SoR.
        if (ability is not SongOfRenewalAOE)
        {
            return;
        }
        
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        heal /= data.DamageModifier;
    }
}
