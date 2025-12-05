using System;
using UnityEngine;

[Serializable]
public abstract class StatusEffect
{
    [SerializeField] private string _name;
    public string Name => _name;
    public int Duration { get; set; }
    
    protected Character Character { get; private set; }
    protected StatusEffectManager Manager { get; private set; }
    
    [SerializeField] private StatusEffectData _data;
    public StatusEffectData Data => _data;
    
    protected StatusEffect(int duration = 3)
    {
        Duration = duration;

        _data = StatusEffectDataRegistry.GetDataForType(GetType());
        
        _name = Data.Name;
    }

    public void Initialize(Character character, StatusEffectManager manager)
    {
        Character = character;
        Manager = manager;
        
        OnApply();
    }

    public virtual void IncreaseDuration(int amount = 1)
    {
        Duration = Mathf.Max(Duration, amount);
    }

    public void DecreaseDuration(int amount = 1)
    {
        Duration -= amount;
        if (Duration <= 0)
        {
            Manager.RemoveStatusEffect(this);
        }
    }

    /// <summary>
    /// Decrements duration and returns whether status effect is still active.
    /// </summary>
    /// <returns>Whether the status effect is still active.</returns>
    public bool TickDuration()
    {
        if (Data.IsPermanent)
        {
            return true;
        }
        return --Duration > 0;
    }
    
    // Virtual methods. Overriden and implemented in subclasses as needed.
    public virtual void OnApply() {}
    public virtual void OnExpire() {}
    public virtual void OnTurnStart() {}
    public virtual void OnTurnEnd() {}
    public virtual void OnCardPlayed(Card card) {}
    public virtual void OnTargetedByCard() {}
    public virtual void OnBurnApplied() {}
    public virtual void OnCombatEnded() {}
    public virtual void ModifyIncomingDamage(ref float damage, Ability ability) {}
    public virtual void ModifyOutgoingDamage(ref float damage, Ability ability) {}
    public virtual void ModifyIncomingHeal(ref float heal, Ability ability) {}
    public virtual void ModifyOutgoingHeal(ref float heal, Ability ability) {}
    public virtual void ModifyBurnDamage(ref int damage) {}
    public virtual void ModifyBurnApplicationChance(ref float chance) {}
    public virtual void ModifyStunApplicationChance(ref float chance) {}
}
