using UnityEngine;

public class WitheringRecovery : Trait
{
    public override void ModifyIncomingHeal(ref float heal, Ability ability)
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        heal /= data.DamageModifier;
    }
}
