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

        var modifier = 1f - data.DamageModifierPercent / 100f;
        heal *= modifier;
    }
}
