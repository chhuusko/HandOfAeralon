using UnityEngine;

public class BulwarksThreshold : Trait
{
    private bool _effectApplied;
    
    public BulwarksThreshold(int duration) : base(duration)
    {
    }

    public override void OnStartCombat()
    {
        _effectApplied = false;
    }

    public override void OnTakeDamage()
    {
        if (_effectApplied)
        {
            return;
        }
        
        // Cast to avoid loss of fraction.
        if ((float)Character.GetCurrentHealth() / Character.GetMaxHealth() < 0.5f)
        {
            _effectApplied = true;
            Character.GetStatusEffectManager().AddStatusEffect(new Fortified(2));
        }
    }
}
