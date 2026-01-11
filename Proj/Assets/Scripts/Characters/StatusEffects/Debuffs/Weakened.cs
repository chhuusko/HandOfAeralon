using UnityEngine;

public class Weakened : StatusEffect
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Weakened(int duration) : base(duration)
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
        combinedModifier -= modifier;
    }
}
