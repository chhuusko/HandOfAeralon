using UnityEngine;

public class Burn : StatusEffect
{
    public Burn(int duration) : base(duration)
    {
    }

    public override void OnTurnStart()
    {
        Character.TakeDamage(4);
    }
}
