using UnityEngine;

public class Weakened : StatusEffect
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Weakened(int duration) : base(duration)
    {
    }

    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        damage /= 1.5f;
    }
}
