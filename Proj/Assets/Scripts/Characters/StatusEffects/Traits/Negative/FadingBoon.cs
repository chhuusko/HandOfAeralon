using UnityEngine;

public class FadingBoon : Trait
{
    private bool _effectApplied;

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

        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }
        
        _effectApplied = true;
        statusEffect.Duration = Mathf.FloorToInt(statusEffect.Duration / data.DamageModifier);
        if (statusEffect.Duration <= 0)
        {
            Manager.RemoveStatusEffect(statusEffect);
        }
    }
}
