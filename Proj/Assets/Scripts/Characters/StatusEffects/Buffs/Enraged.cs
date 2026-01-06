using UnityEngine;

public class Enraged : StatusEffect
{
    public Enraged(int duration) : base(duration)
    {
    }
    
    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        var modifier = 1f + data.DamageModifierPercent / 100f;
        damage *= modifier;
    }
}
