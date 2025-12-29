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

        if (Manager.ContainsStatusEffect<Tremor>())
        {
            tremor = (Tremor)Manager.GetStatusEffect<Tremor>();
        }
        else
        {
            tremor = new Tremor(1);
            Manager.AddStatusEffect(tremor);
        }

        tremor.IncreaseStacks();
    }
}
