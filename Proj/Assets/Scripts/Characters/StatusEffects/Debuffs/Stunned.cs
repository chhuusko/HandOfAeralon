using UnityEngine;

public class Stunned : StatusEffect
{
    public Stunned(int duration) : base(duration)
    {
    }

    public override void OnApply()
    {
        Character.CanMove = false;
        Character.CanAttack = false;
    }

    public override void OnExpire()
    {
        Character.CanMove = true;
        Character.CanAttack = true;
    }
}
