using UnityEngine;

public class UnyieldingTremor : Trait
{
    private float _totalDamageReduction = 1;
    
    public override void OnStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        if (caster != Character || target.GetFaction() != Faction.Enemy || statusEffect is not Slowed or Stunned)
        {
            return;
        }

        var data = Data as UnyieldingTremorData;

        if (!data)
        {
            return;
        }
        
        _totalDamageReduction = Mathf.Min(_totalDamageReduction + data.Modifier, data.Cap);
    }

    public override void ModifyIncomingDamage(ref float damage, Ability ability)
    {
        damage /= _totalDamageReduction;
    }
}
