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

    public override void ModifyOutgoingDamage(ref float damage, ref float combinedModifier, Ability ability)
    {
        var modifier = _combinedDamageModifier / 100f;
        combinedModifier += modifier;
    }

    public override void OnCombatEnded()
    {
        System.RemoveStatusEffect(this);
    }
}
