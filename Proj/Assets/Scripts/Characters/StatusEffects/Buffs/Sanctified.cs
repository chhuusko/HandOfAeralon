using UnityEngine;

public class Sanctified : StatusEffect
{
    public override void ModifyIncomingDamage(ref float damage, Ability ability)
    {
        if (damage > 0f)
        {
            damage = 0f;
            Manager.RemoveStatusEffect(this);
        }
    }
}
