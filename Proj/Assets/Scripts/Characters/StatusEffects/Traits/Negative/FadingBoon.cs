using UnityEngine;

public class FadingBoon : Trait
{
    private bool _effectApplied;

    public override void ResetCombatState()
    {
        _effectApplied = false;
    }

    public override bool BeforeStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        if (target != Character)
        {
            return true;
        }
        
        // Trait applies only to buffs.
        if (_effectApplied || statusEffect.Data.Type is not StatusEffectType.Buff)
        {
            return true;
        }

        var data = Data as DamageModifyingData;

        if (!data)
        {
            return true;
        }
        
        _effectApplied = true;
        statusEffect.SetDuration(Mathf.FloorToInt(statusEffect.Duration / data.DamageModifier));
        return statusEffect.Duration > 0;
    }
}
