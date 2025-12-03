using UnityEngine;

public class Trait : StatusEffect
{
    public virtual void OnStartCombat() {}
    public virtual void OnTakeDamage() {}
    public virtual void OnAbilityUsed(Ability ability) {}
}
