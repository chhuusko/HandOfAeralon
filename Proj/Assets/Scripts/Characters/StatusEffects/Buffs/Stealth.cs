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
        Character.Data.IncreaseBaseMovementPoints(data.MovementPointModifier);
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
        Character.Data.DecreaseBaseMovementPoints(data.MovementPointModifier);
        Character.DecreaseCurrentMovementPoints();
    }

    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        var data = Data as StealthData;

        if (!data)
        {
            return;
        }

        damage *= data.DamageModifier;
    }

    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        if (abilityData.Damage > 0)
        {
            ShouldExpire = true;
        }
    }
    
    // Effect breaks on taking damage.
    public override void OnTakeDamage()
    {
        ShouldExpire = true;
    }
}
