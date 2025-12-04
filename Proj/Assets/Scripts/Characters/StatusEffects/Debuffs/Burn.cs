using System;
using UnityEngine;

public class Burn : StatusEffect
{
    public Burn(int duration) : base(duration)
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
        
        Character.TakeDamage(data.Damage);
    }
}
