using UnityEngine;

public class WitheringRecovery : Trait
{
    public override void ModifyIncomingHeal(ref float heal, ref float combinedModifier, Ability ability)
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        var modifier = data.DamageModifierPercent / 100f;
        combinedModifier -= modifier;
    }
}
