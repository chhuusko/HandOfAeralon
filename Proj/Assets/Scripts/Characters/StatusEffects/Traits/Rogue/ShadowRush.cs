using UnityEngine;

public class ShadowRush : Trait
{
    public override void OnStatusEffectApplied(Character caster, StatusEffect statusEffect)
    {
        if (statusEffect is not Stealth)
        {
            return;
        }
        
        AddHaste();
    }

    public override void OnStatusEffectRemoved(StatusEffect statusEffect)
    {
        if (statusEffect is not Stealth)
        {
            return;
        }
        
        AddHaste();
    }

    private void AddHaste()
    {
        var data = Data as IntModifierData;

        if (!data)
        {
            return;
        }
        
        Manager.AddStatusEffect(new Haste(data.Modifier));
    }
}
