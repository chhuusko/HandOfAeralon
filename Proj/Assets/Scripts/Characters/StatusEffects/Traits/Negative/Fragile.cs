using UnityEngine;

public class Fragile : Trait
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

        float healthPercent = (float)Character.GetCurrentHealth() / Character.GetMaxHealth();
        if (healthPercent >= data.ThresholdPercent / 100f)
        {
            return;
        }
        _effectApplied = true;
        Manager.AddStatusEffect(new Vulnerable(data.TurnAmount));
    }
}
