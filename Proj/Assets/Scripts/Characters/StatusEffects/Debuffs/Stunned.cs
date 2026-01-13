using UnityEngine;

public class Stunned : StatusEffect
{
    public Stunned(int duration) : base(duration)
    {
    }

    public override void OnApply()
    {
        Character.IsStunned = true;
        Character.SpawnStunnedEffect();
    }

    public override void OnExpire()
    {
        Character.IsStunned = false;
        Character.DestroyStunnedEffect();
    }
}
