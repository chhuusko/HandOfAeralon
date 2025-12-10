using UnityEngine;

public class Giantblood : Trait
{
    private int _maxHealth;
    
    public override void OnApply()
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }
        
        _maxHealth = Character.GetMaxHealth();
        
        Character.SetBaseHealthPoints(Mathf.RoundToInt(_maxHealth * data.DamageModifier));
    }
}
