using UnityEngine;

public class BattlebornMomentum : Trait
{
    private int _abilitiesUsed;
    private bool _effectApplied;
    
    public BattlebornMomentum(int duration) : base(duration)
    {
    }

    public override void OnStartCombat()
    {
        _effectApplied = false;
        _abilitiesUsed = 0;
    }

    public override void OnAbilityUsed(Ability ability)
    {
        var data = Data as IntThresholdData;

        if (!data)
        {
            return;
        }
        
        if (++_abilitiesUsed >= 3 && !_effectApplied)
        {
            _abilitiesUsed = 0;
            _effectApplied = true;
            
            Manager.AddStatusEffect(new Empowered(data.TurnAmount));
        }
    }
}
