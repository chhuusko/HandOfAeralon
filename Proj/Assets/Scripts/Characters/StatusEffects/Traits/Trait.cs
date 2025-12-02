using UnityEngine;

public class Trait : StatusEffect
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Trait(int duration) : base(duration)
    {
    }

    public virtual void OnStartCombat() {}
    public virtual void OnTakeDamage() {}
    public virtual void OnAbilityUsed(Ability ability) {}
}
