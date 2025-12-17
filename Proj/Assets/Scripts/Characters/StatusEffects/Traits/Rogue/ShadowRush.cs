using UnityEngine;

public class ShadowRush : Trait
{
    public override bool OnTryApplyStatusEffect(Character caster, Character target, StatusEffect statusEffect)
    {
        if (target != Character)
        {
            return true;
        }
        
        if (statusEffect is not Stealth)
        {
            return true;
        }
        
        AddHaste();
        
        return true;
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
