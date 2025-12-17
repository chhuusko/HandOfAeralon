using UnityEngine;

public class Trait : StatusEffect
{
    public virtual void OnCombatStarted() {}
    public virtual void OnTakeDamage() {}
    public virtual void OnAbilityUsed(AbilityExecutionData abilityData) {}
    public virtual void OnStatusEffectRemoved(StatusEffect statusEffect) {}
    public virtual void ModifyAoE(ref int AoE) {}
    public virtual void ModifyDerivedStats(ref float hpFactor, ref float damageFactor) {}
}
