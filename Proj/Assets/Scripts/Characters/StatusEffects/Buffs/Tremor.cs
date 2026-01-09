using UnityEngine;

public class Tremor : StatusEffect
{
    private float _totalDamageReduction;

    public Tremor(int duration) : base(duration)
    {
    }

    public void IncreaseStacks()
    {
        var data = Data as UnyieldingTremorData;

        if (!data)
        {
            return;
        }
        
        // Stacks can never be more than the cap.
        _totalDamageReduction = Mathf.Min(_totalDamageReduction + data.ModifierPercent, data.CapPercent);
    }

    public override void ModifyIncomingDamage(ref float damage, ref float combinedModifier, Ability ability)
    {
        var modifier = _totalDamageReduction / 100f;
        combinedModifier -= modifier;
    }
}
