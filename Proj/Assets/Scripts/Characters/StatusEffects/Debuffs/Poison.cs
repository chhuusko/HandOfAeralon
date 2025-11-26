using UnityEngine;

public class Poison : StatusEffect
{
    public Poison(int duration) : base(duration)
    {
    }

    public override void IncreaseDuration(int amount = 1)
    {
        Duration += amount;
    }

    public override void OnTurnStart()
    {
        Character.TakeDamage(Duration);
    }
}
