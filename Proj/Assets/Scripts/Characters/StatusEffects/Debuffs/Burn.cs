using System;
using UnityEngine;

public class Burn : StatusEffect
{
    private Character _source;

    public Burn(int duration = 3) : base(duration)
    {
        
    }
    
    public Burn(Character source, int duration = 3) : base(duration)
    {
        _source = source;
    }

    public override void IncreaseDuration(int amount = 1)
    {
        SetDuration(Duration + amount);
    }

    public override void OnApply()
    {
        System.OnBurnApplied(Character);
    }

    public override void OnTurnStart()
    {
        var data = Data as DamageData;
        if (!data)
        {
            return;
        }
        
        float damage = data.Damage;
        if (_source != null)
        {
            damage = _source.GetStatusEffectManager().ModifyOutgoingBurnDamage(damage);
        }
        damage = Character.GetStatusEffectManager().ModifyIncomingBurnDamage(damage);

        Character.TakeDamage(Mathf.RoundToInt(damage));
        CombatEventManager.InvokeOnStatusEffectDamageDealt(Character, this, Mathf.RoundToInt(damage));
    }
}
