using System;
using UnityEngine;

public class Burn : StatusEffect
{
    private Character _source;
    
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
        Manager.OnBurnApplied(Character);
    }

    public override void OnTurnStart()
    {
        var data = Data as DamageData;

        if (!data)
        {
            return;
        }
        
        var baseDamage = data.Damage;
        var finalDamage = (_source != null) ? _source.GetStatusEffectManager().ApplyBurnDamageModifiers(baseDamage)
            : baseDamage;

        Character.TakeDamage(Mathf.RoundToInt(finalDamage));
    }
}
