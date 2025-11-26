using UnityEngine;

public class Slowed : StatusEffect
{
    public Slowed(int duration) : base(duration)
    {
    }

    public override void OnApply()
    {
        Character.DecreaseCurrentMovementPoints();
        Character.DecreaseCurrentInitiative();
    }

    public override void OnExpire()
    {
        Character.IncreaseCurrentMovementPoints();
        Character.IncreaseCurrentInitiative();
    }
}
