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

    public virtual void OnApply() {}
    public virtual void OnExpire() {}
    public virtual void OnTurnStart() {}
    public virtual void OnTurnEnd() {}
    public virtual void ModifyIncomingDamage(ref int damage) {}
    public virtual void ModifyOutgoingDamage(ref int damage) {}
}
