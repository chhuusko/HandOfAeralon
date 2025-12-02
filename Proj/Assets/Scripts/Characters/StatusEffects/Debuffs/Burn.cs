using UnityEngine;

public class Burn : StatusEffect
{
    public Burn(int duration) : base(duration)
    {
    }

    public override void OnTurnStart()
    {
        var data = Data as DamageOverTimeData;

        if (!data)
        {
            return;
        }
        
        Character.TakeDamage(data.Damage);
    }
}
