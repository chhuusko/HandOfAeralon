using UnityEngine;

public class SlowedCasting : Trait
{
    private bool _effectApplied;

    public override void ResetCombatState()
    {
        _effectApplied = false;
    }

    public override void OnAbilityCast(Character character, Ability ability)
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
        
        if (Character.IsAbilityCooldownActive(ability))
        {
            Character.ChangeCooldown(ability, data.Modifier);
        }
    }
}
