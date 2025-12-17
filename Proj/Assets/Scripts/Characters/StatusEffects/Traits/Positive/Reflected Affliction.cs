using System;
using System.Linq;
using UnityEngine;

public class ReflectedAffliction : Trait
{
    private bool _effectApplied;

    public override void OnCombatStarted()
    {
        _effectApplied = false;
    }

    public override bool OnTryApplyStatusEffect(Character caster, Character target, StatusEffect statusEffect)
    {
        if (target != Character)
        {
            return true;
        }
        
        if (_effectApplied || !caster || statusEffect.Data.Type is not StatusEffectType.Debuff)
        {
            return true;
        }
        
        _effectApplied = true;
        
        var reflected = statusEffect.Data.CreateInstance(statusEffect.Duration);
        caster.GetStatusEffectManager().AddStatusEffect(reflected, Character);

        // Block adding the effect.
        return false;
    }
}
