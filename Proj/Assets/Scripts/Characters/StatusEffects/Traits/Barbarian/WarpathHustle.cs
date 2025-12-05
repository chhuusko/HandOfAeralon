using UnityEngine;

public class WarpathHustle : Trait
{
    private int _enemiesDamaged;

    public override void OnTurnStart()
    {
        var data = Data as IntModifierData;

        if (!data)
        {
            return;
        }
        
        // Increase movement points by enemies killed, but not more than the specified max.
        Character.IncreaseCurrentMovementPoints(Mathf.Max(_enemiesDamaged, data.Modifier));
        
        _enemiesDamaged = 0;
    }

    public override void OnAbilityUsed(AbilityExecutionData abilityData)
    {
        if (abilityData.Damage <= 0)
        {
            return;
        }

        _enemiesDamaged++;
    }
}
