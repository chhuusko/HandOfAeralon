using UnityEngine;

public class Stealth : StatusEffect
{
    public override void OnApply()
    {
        Character.IsTargetable = false;
    }

    public override void OnExpire()
    {
        Character.IsTargetable = true;
    }

    // Effect breaks on taking damage.
    public override void ModifyIncomingDamage(ref float damage, Ability ability)
    {
        Manager.RemoveStatusEffect(this);
    }
}
