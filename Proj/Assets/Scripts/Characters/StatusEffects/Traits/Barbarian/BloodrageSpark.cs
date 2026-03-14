using UnityEngine;

public class BloodrageSpark : Trait
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
        
        var data = Data as IntModifierData;

        if (!data)
        {
            return;
        }
        
        _effectApplied = true;
        System.AddStatusEffect(new Empowered(data.Modifier));
        System.AddStatusEffect(new Fortified(data.Modifier));
    }
}
