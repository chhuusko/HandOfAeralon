using UnityEngine;

public class Trait : StatusEffect
{
    public virtual void OnCombatStarted() {}
    public virtual void OnTakeDamage() {}
    public virtual void OnAbilityUsed(AbilityExecutionData abilityData) {}
    public virtual void OnStatusEffectApplied(Character caster, StatusEffect statusEffect) {}
}
