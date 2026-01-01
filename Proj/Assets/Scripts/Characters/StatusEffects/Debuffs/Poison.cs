using UnityEngine;

public class Poison : StatusEffect
{
    public Poison(int duration) : base(duration)
    {
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
        Character.TakeDamage(Duration);
    }
}
