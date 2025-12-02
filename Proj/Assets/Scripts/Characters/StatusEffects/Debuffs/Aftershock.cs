using UnityEngine;

public class Aftershock : StatusEffect
{
    public override void OnCardPlayed()
    {
        var data = Data as DamageData;

        if (!data)
        {
            return;
        }
        
        Character.TakeDamage(data.Damage);
    }
}
