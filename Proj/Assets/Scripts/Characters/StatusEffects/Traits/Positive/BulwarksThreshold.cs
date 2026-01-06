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
        float healthPercent = (float)Character.GetCurrentHealth() / Character.GetMaxHealth();
        if (healthPercent < data.ThresholdPercent / 100f)
        {
            _effectApplied = true;
            Character.GetStatusEffectManager().AddStatusEffect(new Fortified(data.TurnAmount));
        }
    }
}
