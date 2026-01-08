using UnityEngine;

public class WarpathHustle : Trait
{
    private int _enemiesDamaged;
    private int _originalMovementPoints;
    private float _totalDamageModifier;
    
    public override void ResetCombatState()
    {
        // Reset for next combat.
        _enemiesDamaged = 0;
    }

    public override void OnTurnStart()
    {
        var data = Data as WarpathHustleData;
        if (!data)
        {
            return;
        }
        
        _originalMovementPoints = Character.GetBaseMovementPoints();
        
        // Increase movement points by enemies killed, but not more than the specified max.
        int movementPoints = Mathf.Min(_enemiesDamaged * data.MovementPointModifier,
            data.MovementPointCapacity);
        
        Character.Data.IncreaseBaseMovementPoints(movementPoints);
        Character.IncreaseCurrentMovementPoints(movementPoints);
        
        _totalDamageModifier = Mathf.Min(1f + _enemiesDamaged * (data.DamageModifierPercent / 100f), 
            1f + data.DamageModifierCapacity / 100f);
        
        _enemiesDamaged = 0;
    }

    public override void OnTurnEnd()
    {
        Character.Data.SetBaseMovementPoints(_originalMovementPoints);
    }

    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        if (abilityData.Damage <= 0 || abilityData.Target?.GetFaction() == Character.GetFaction())
        {
            return;
        }

        _enemiesDamaged++;
    }

    public override void ModifyOutgoingDamage(ref float damage, ref float combinedModifier, Ability ability)
    {
        combinedModifier += _totalDamageModifier;
    }
}
