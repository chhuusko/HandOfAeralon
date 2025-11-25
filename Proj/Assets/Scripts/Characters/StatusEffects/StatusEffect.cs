using UnityEngine;

public abstract class StatusEffect
{
    public int Duration { get; private set; }
    
    protected Character Character { get; private set; }
    protected StatusEffectManager Manager { get; private set; }
    
    private StatusEffectData Data;
    
    protected StatusEffect(int duration)
    {
        Duration = duration;

        Data = StatusEffectDataRegistry.GetDataForType(GetType());
    }

    public void Initialize(Character character, StatusEffectManager manager)
    {
        Character = character;
        Manager = manager;
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

    // Each subclass has to set the status effect data.
    // public abstract void SetData(StatusEffectData data);
    
    // Virtual methods. Overriden and implemented in subclasses.
    public virtual void OnApply() {}
    public virtual void OnExpire() {}
    public virtual void OnTurnStart() {}
    public virtual void OnTurnEnd() {}
    public virtual void ModifyIncomingDamage(ref float damage, Ability ability) {}
    public virtual void ModifyOutgoingDamage(ref float damage, Ability ability) {}
}
