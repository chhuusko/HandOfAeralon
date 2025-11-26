using UnityEngine;

public class Fortified : StatusEffect
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Fortified(int duration) : base(duration)
    {
    }

    public override void ModifyIncomingDamage(ref float damage, Ability ability)
    {
        damage /= 1.5f;
    }
}
