using UnityEngine;

public class CrescendoSmash : Trait
{
    private float _totalDamageModifier = 1;

    public override void OnCombatStarted()
    {
        _totalDamageModifier = 1;
    }

    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        if (ability is not LuteSmash_SingleTarget)
        {
            return;
        }

        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }
        
        damage *= _totalDamageModifier;
        
        _totalDamageModifier += data.DamageModifier;
    }
}
