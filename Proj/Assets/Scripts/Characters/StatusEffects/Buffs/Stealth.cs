using UnityEngine;

public class Stealth : StatusEffect
{
    public Stealth(int duration)
    {
    }
    
    public override void OnApply()
    {
        Character.IsTargetable = false;
        Character.gameObject.GetComponent<Renderer>().material.SetFloat("_Camo", 1);
    }

    public override void OnExpire()
    {
        Character.IsTargetable = true;
        Character.gameObject.GetComponent<Renderer>().material.SetFloat("_Camo", 0);
    }

    // Effect breaks on taking damage.
    public override void ModifyIncomingDamage(ref float damage, Ability ability)
    {
        ShouldExpire = true;
    }
}
