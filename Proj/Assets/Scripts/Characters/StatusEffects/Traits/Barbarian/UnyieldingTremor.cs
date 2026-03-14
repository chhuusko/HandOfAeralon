using UnityEngine;

public class UnyieldingTremor : Trait
{
    public override void OnStatusEffectApplied(Character caster, Character target, StatusEffect statusEffect)
    {
        if (caster != Character || target?.GetFaction() == caster.GetFaction() || statusEffect is not Slowed or Stunned)
        {
            return;
        }

        Tremor tremor;

        if (System.ContainsStatusEffect<Tremor>())
        {
            tremor = (Tremor)System.GetStatusEffect<Tremor>();
        }
        else
        {
            tremor = new Tremor(1);
            System.AddStatusEffect(tremor);
        }

        tremor.IncreaseStacks();
    }
}
