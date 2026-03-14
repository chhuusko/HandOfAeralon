using UnityEngine;

public class ShadowRush : Trait
{
    public override bool BeforeStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        if (target != Character)
        {
            return true;
        }
        
        if (statusEffect is not Stealth)
        {
            return true;
        }

        AddBuffs();
        return true;
    }

    public override void OnStatusEffectRemovedFromThis(StatusEffect statusEffect)
    {
        if (statusEffect is not Stealth)
        {
            return;
        }
        
        AddBuffs();
    }
    
    private void AddBuffs()
    {
        var data = Data as IntModifierData;
        if (!data)
        {
            return;
        }

        System.AddStatusEffect(new Haste(data.Modifier));
        System.AddStatusEffect(new Empowered(data.Modifier));
    }
}
