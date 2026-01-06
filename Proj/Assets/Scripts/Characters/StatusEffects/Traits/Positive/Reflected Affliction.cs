using System;
using System.Linq;
using UnityEngine;

public class ReflectedAffliction : Trait
{
    private bool _effectApplied;

    public override void ResetCombatState()
    {
        _effectApplied = false;
    }

    public override bool BeforeStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        if (statusEffect.IsReflected)
        {
            return true;
        }
        
        if (target != Character || statusEffect.Data.Type is not StatusEffectType.Debuff || !caster ||
            caster.GetFaction() == Character.GetFaction() || _effectApplied)
        {
            return true;
        }
        
        _effectApplied = true;
        
        // Reflect back to caster.
        var reflected = statusEffect.Data.CreateInstance(statusEffect.Duration);
        reflected.IsReflected = true;
        caster.GetStatusEffectManager().AddStatusEffect(reflected, Character);

        // Block adding the effect.
        return false;
    }
}
