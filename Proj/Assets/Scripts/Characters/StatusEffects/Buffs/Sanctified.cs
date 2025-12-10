using System.Collections;
using UnityEngine;

public class Sanctified : StatusEffect
{
    public Sanctified(int duration) : base(duration)
    {
    }
    
    public override void ModifyIncomingDamage(ref float damage, Ability ability)
    {
        if (damage > 0f)
        {
            damage = 0f;
            ShouldExpire = true;
        }
    }
}
