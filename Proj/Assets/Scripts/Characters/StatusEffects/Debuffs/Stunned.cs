using UnityEngine;

public class Stunned : StatusEffect
{
    public Stunned(int duration) : base(duration)
    {
    }

    public override void OnApply()
    {
        Character.IsStunned = true;
    }

    public override void OnExpire()
    {
        Character.IsStunned = false;
    }
}
