using UnityEngine;

public abstract class StatusEffect
{
    public int Duration { get; private set; }
    
    private StatusEffectData _statusEffectData;
    private Character _character;
    private int _stacks;
    
    protected StatusEffect(Character character, int duration)
    {
        _character = character;
        Duration = duration;
    }

    /// <summary>
    /// Decrements duration and returns whether status effect is still active.
    /// </summary>
    /// <returns>Whether the status effect is still active.</returns>
    public bool TickDuration()
    {
        if (_statusEffectData.IsPermanent)
        {
            return true;
        }
        return --Duration > 0;
    }

    // Each subclass has to set the status effect data.
    public abstract void SetData(StatusEffectData data);
    
    // Virtual methods. Overriden and implemented in subclasses.
    public virtual void OnApply() {}
    public virtual void OnExpire() {}
    public virtual void OnTurnStart() {}
    public virtual void OnTurnEnd() {}
    public virtual void ModifyIncomingDamage(ref float damage) {}
    public virtual void ModifyOutgoingDamage(ref float damage) {}
}
