using UnityEngine;

public class Enraged : StatusEffect
{
    public Enraged(int duration) : base(duration)
    {
    }
    
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
