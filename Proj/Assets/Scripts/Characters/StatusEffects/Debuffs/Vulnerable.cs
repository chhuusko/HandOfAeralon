using UnityEngine;

public class Vulnerable : StatusEffect
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vulnerable(int duration) : base(duration)
    {
    }

    public override void ModifyIncomingDamage(ref float damage, Ability ability)
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }
        
        damage *= data.DamageModifier;
    }
}
