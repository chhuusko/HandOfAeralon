using System.Collections;
using UnityEngine;

public class Sanctified : StatusEffect
{
    public Sanctified(int duration) : base(duration)
    {
    }

    public override bool BeforeStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        // Sanctified disallows receiving debuffs.
        return statusEffect.Data.Type is not StatusEffectType.Debuff;
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
