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
        // Trait applies only to buffs.
        if (_effectApplied || statusEffect.Data.Type is not StatusEffectType.Buff)
        {
            return;
        }

        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }
        
        _effectApplied = true;
        statusEffect.SetDuration(Mathf.FloorToInt(statusEffect.Duration / data.DamageModifier));
        if (statusEffect.Duration <= 0)
        {
            Manager.RemoveStatusEffect(statusEffect);
        }
    }
}
