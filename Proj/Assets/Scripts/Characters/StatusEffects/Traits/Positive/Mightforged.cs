using UnityEngine;

public class Mightforged : Trait
{
    public override void ModifyOutgoingDamage(ref float damage, ref float combinedModifier, Ability ability)
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        var modifier = data.DamageModifierPercent / 100f;
        combinedModifier += modifier;
    }
}
