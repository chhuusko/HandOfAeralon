using UnityEngine;

public class ConduitOfPower : StatusEffect
{
    private float _combinedDamageModifier = 1;

    public override void OnTargetedByCard()
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }
        
        _combinedDamageModifier += data.DamageModifier;
        
    }

    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        damage *= _combinedDamageModifier;
    }

    public override void OnCombatEnded()
    {
        Manager.RemoveStatusEffect(this);
    }
}
