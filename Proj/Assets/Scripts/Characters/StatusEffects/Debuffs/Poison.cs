using UnityEngine;

public class Poison : StatusEffect
{
    private Character _source;

    public Poison(int duration = 3) : base(duration)
    {
        
    }
    
    public Poison(Character source, int duration = 3) : base(duration)
    {
        _source = source;
    }

    public override void IncreaseDuration(int amount = 1)
    {
        var data = Data as IntCapData;

        if (!data)
        {
            return;
        }
        
        // Stacks can never be more than the cap.
        SetDuration(Mathf.Min(Duration + amount, data.Cap));
    }

    public override void OnTurnStart()
    {
        float damage = Duration;
        if (_source != null)
        {
            damage = _source.GetStatusEffectManager().ModifyOutgoingPoisonDamage(damage);
        }
        damage = Character.GetStatusEffectManager().ModifyIncomingPoisonDamage(damage);

        Character.TakeDamage(Mathf.RoundToInt(damage));
    }
}
