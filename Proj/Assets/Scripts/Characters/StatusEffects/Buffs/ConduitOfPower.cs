using UnityEngine;

public class ConduitOfPower : StatusEffect
{
    private float _combinedDamageModifier;

    public override void OnTargetedByCard()
    {
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }
        
        _combinedDamageModifier += data.DamageModifierPercent;
    }

    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        var modifier = 1f + _combinedDamageModifier / 100f;
        damage *= modifier;
    }

    public override void OnCombatEnded()
    {
        Manager.RemoveStatusEffect(this);
    }
}
