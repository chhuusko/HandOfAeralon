using System;
using UnityEngine;

public class ReflectedAffliction : Trait
{
    private bool _effectApplied;
    private Character _caster;

    public override void OnCombatStarted()
    {
        _effectApplied = false;
    }

    public override void OnStatusEffectApplied(Character caster, StatusEffect statusEffect)
    {
        if (_effectApplied)
        {
            return;
        }
        
        if (!caster)
        {
            return;
        }
        
        Manager.RemoveStatusEffect(statusEffect);
        caster.GetStatusEffectManager().AddStatusEffect(statusEffect);
        _effectApplied = true;
    }
}
