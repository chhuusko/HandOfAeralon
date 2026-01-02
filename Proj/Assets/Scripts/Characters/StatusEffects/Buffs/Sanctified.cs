using System.Collections;
using UnityEngine;

public class Sanctified : StatusEffect
{
    public Sanctified(int duration) : base(duration)
    {
    }
    
    public override void ModifyIncomingDamage(ref float damage, Ability ability)
    {
        damage = 0f;
    }

    public override void OnTakeDamage()
    {
        ShouldExpire = true;
    }
}
