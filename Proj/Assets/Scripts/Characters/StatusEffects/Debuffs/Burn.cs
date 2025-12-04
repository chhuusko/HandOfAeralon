using System;
using UnityEngine;

public class Burn : StatusEffect
{
    private Character _source;
    
    public Burn(int duration, Character source) : base(duration)
    {
        _source = source;
    }

    public override void OnApply()
    {
        Manager.OnBurnApplied(Character);
    }

    public override void OnTurnStart()
    {
        var data = Data as DamageData;

        if (!data)
        {
            return;
        }
        
        var damage = data.Damage;

        if (_source != null)
        {
            _source.GetStatusEffectManager().ApplyBurnDamageModifiers(damage);
        }
        
        Character.TakeDamage(damage);
    }
}
