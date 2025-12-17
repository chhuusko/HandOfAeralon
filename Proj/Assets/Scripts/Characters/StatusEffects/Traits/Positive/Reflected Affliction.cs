using System;
using UnityEngine;

public class ReflectedAffliction : Trait
{
    private bool _effectApplied;

    public override void OnCombatStarted()
    {
        _effectApplied = false;
    }

    public override void OnStatusEffectApplied(Character caster, StatusEffect statusEffect)
    {
        if (_effectApplied || !caster || statusEffect.Data.Type is not StatusEffectType.Debuff)
        {
            return;
        }
        
        Debug.Log("Effect Applied!");
        
        Manager.RemoveStatusEffect(statusEffect);
        caster.GetStatusEffectManager().AddStatusEffect(statusEffect);
        _effectApplied = true;
    }
}
