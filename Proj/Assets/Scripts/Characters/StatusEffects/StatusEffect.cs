using System;
using UnityEngine;

[Serializable]
public abstract class StatusEffect
{
    [SerializeField] private string _name;
    public string Name => _name;
    [SerializeField] private int _duration;
    public int Duration => _duration;
    
    [SerializeField] private StatusEffectData _data;
    public StatusEffectData Data => _data;
    
    public bool ShouldExpire;
    
    protected Character Character { get; private set; }
    protected StatusEffectManager Manager { get; private set; }

    private bool _skipNextTick;
    
    protected StatusEffect(int duration = 3)
    {
        _duration = duration;

        _data = StatusEffectDataRegistry.GetDataForType(GetType());

        if (!_data)
        {
            Debug.LogError($"{GetType()} has no status effect data");
        }

        if (string.IsNullOrEmpty(Data.Name))
        {
            Debug.LogError($"{GetType()} has no status effect name");
        }
        
        _name = Data.Name;
    }

    public void Setup(Character character, StatusEffectManager manager)
    {
        Character = character;
        Manager = manager;
    }

    public virtual void Initialize()
    {
        // If the status effect is applied out of turn, it should not tick down at start of next turn.
        _skipNextTick = Character != CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        if (_data && !_data.SkipFirstTick)
        {
            _skipNextTick = false;
        }

        CombatEventManager.OnTryAddStatusEffect += BeforeStatusEffectApplied;
        
        OnApply();
    }

    public void Cleanup()
    {
        CombatEventManager.OnTryAddStatusEffect -= BeforeStatusEffectApplied;
    }
    
    public void SetDuration(int duration) => _duration = duration;

    public virtual void IncreaseDuration(int amount = 1)
    {
        _duration += amount;
    }

    public void DecreaseDuration(int amount = 1)
    {
        _duration -= amount;
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

        if (_skipNextTick)
        {
            _skipNextTick = false;
            return true;
        }
        
        return --_duration > 0;
    }
    
    // Virtual methods. Overriden and implemented in subclasses as needed.
    public virtual bool BeforeStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        return true;
    }
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
