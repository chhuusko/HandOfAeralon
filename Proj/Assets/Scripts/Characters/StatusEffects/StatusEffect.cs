using UnityEngine;

public abstract class StatusEffect
{
    public int Duration { get; private set; }
    
    private StatusEffectData _buffData;
    private Character _character;
    private int _stacks;
    
    protected StatusEffect(Character character, StatusEffectData buffData)
    {
        _character = character;
        _buffData = buffData;
        Duration = buffData.Duration;
    }

    /// <summary>
    /// Decrements duration and returns whether status effect is still active.
    /// </summary>
    /// <returns>Whether the status effect is still active.</returns>
    public bool TickDuration()
    {
        if (_buffData.IsPermanent)
        {
            return true;
        }
        return --Duration > 0;
    }
    
    // Virtual methods. Overriden and implemented in subclasses.
    public virtual void OnApply() {}
    public virtual void OnExpire() {}
    public virtual void OnTurnStart() {}
    public virtual void OnTurnEnd() {}
    public virtual void ModifyIncomingDamage(ref int damage) {}
    public virtual void ModifyOutgoingDamage(ref int damage) {}
}
