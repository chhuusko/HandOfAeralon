using UnityEngine;

public class BulwarksThreshold : Trait
{
    private bool _effectApplied;

    public override void ResetCombatState()
    {
        _effectApplied = false;
    }

    public override void OnTakeDamage()
    {
        if (_effectApplied)
        {
            return;
        }
        
        var data = Data as FloatThresholdData;

        if (!data)
        {
            return;
        }
        
        // Cast to avoid loss of fraction.
        if ((float)Character.GetCurrentHealth() / Character.GetMaxHealth() < data.Threshold)
        {
            _effectApplied = true;
            Character.GetStatusEffectManager().AddStatusEffect(new Fortified(data.TurnAmount));
        }
    }
}
