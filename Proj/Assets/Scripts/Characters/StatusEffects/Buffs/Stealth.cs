using UnityEngine;

public class Stealth : StatusEffect
{
    public Stealth(int duration)
    {
    }
    
    public override void OnApply()
    {
        var data = Data as StealthData;

        if (!data)
        {
            return;
        }
        
        Character.IsTargetable = false;
        Character.gameObject.GetComponent<Renderer>().material.SetFloat("_Camo", 1);
        Character.Data.SetBaseMovementPoints(Character.GetBaseMovementPoints() + data.MovementPointModifier);
        Character.IncreaseCurrentMovementPoints();
    }

    public override void OnExpire()
    {
        var data = Data as StealthData;

        if (!data)
        {
            return;
        }
        
        Character.IsTargetable = true;
        Character.gameObject.GetComponent<Renderer>().material.SetFloat("_Camo", 0);
        Character.Data.SetBaseMovementPoints(Character.GetBaseMovementPoints() - data.MovementPointModifier);
        Character.DecreaseCurrentMovementPoints();
    }

    // Effect breaks on taking damage.
    public override void ModifyIncomingDamage(ref float damage, Ability ability)
    {
        ShouldExpire = true;
    }

    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        ShouldExpire = true;
        
        var data = Data as StealthData;

        if (!data)
        {
            return;
        }

        damage *= data.DamageModifier;
    }
}
