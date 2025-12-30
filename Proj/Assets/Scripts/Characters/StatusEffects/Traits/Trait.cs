using UnityEngine;

public class Trait : StatusEffect
{
    public override void Initialize()
    {
        base.Initialize();
        ResetCombatState();
    }
    public virtual void ResetCombatState() {}
    public virtual void OnCombatStarted() {}
    public virtual void OnTakeDamage() {}
    public virtual void OnDeath(Character c) {}
    public virtual void OnAbilityUsed(AbilityExecutionData abilityData) {}
    public virtual void OnStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect) {}
    public virtual void OnStatusEffectRemovedFromThis(StatusEffect statusEffect) {}
    public virtual void OnStatusEffectRemovedFromAny(Character character, StatusEffect statusEffect) {}
    public virtual void ModifyAoE(ref int AoE) {}
    public virtual void ModifyDerivedStats(ref float hpFactor, ref float damageFactor) {}
    public override void OnCombatEnded()
    {
        ResetCombatState();
    }
}
