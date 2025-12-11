using UnityEngine;

public class Giantblood : Trait
{
    private int _maxHealth;
    
    // public override void OnApply()
    // {
    //     var data = Data as DamageModifyingData;
    //
    //     if (!data)
    //     {
    //         return;
    //     }
    //     
    //     _maxHealth = Character.GetMaxHealth();
    //     
    //     Character.SetDerivedHealthPoints(Mathf.RoundToInt(_maxHealth * data.DamageModifier));
    // }

    public override void ModifyDerivedStats(ref float hpFactor, ref float damageFactor)
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }
        
        hpFactor *= data.DamageModifier;
    }
}
