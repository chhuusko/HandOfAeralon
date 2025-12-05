using UnityEngine;

public class Fragile : Trait
{
    private bool _effectApplied;

    public override void OnCombatStarted()
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

        if (Character.GetCurrentHealth() >= data.Threshold)
        {
            return;
        }
        _effectApplied = true;
        Manager.AddStatusEffect(new Vulnerable(data.TurnAmount));
    }
}
