using UnityEngine;

public class Aftershock : StatusEffect
{
    public Aftershock(int duration) : base(duration)
    {
    }
    
    public override void OnCardPlayed(Card card)
    {
        var data = Data as DamageData;
        if (!data)
        {
            return;
        }
        
        Character.TakeDamage(data.Damage);
        CombatEventManager.InvokeOnStatusEffectDamageDealt(Character, this, data.Damage);
    }
}
