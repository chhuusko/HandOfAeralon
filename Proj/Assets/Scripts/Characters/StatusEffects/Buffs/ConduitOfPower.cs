using UnityEngine;

public class ConduitOfPower : StatusEffect
{
    float combinedDamageModifier;

    public override void OnCardPlayed()
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }
        
        combinedDamageModifier += data.DamageModifier;
    }

    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        damage *= combinedDamageModifier;
    }
}
