using UnityEngine;

public class Empowered : StatusEffect
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Empowered(int duration) : base(duration)
    {
    }

    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        var data = Data as DamageModifyingData;
        damage *= data.DamageModifier;
    }
}
