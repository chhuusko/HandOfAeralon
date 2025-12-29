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
        _totalDamageReduction = Mathf.Min(_totalDamageReduction + data.Modifier, data.Cap);
    }

    public override void ModifyIncomingDamage(ref float damage, Ability ability)
    {
        damage *= 1f - _totalDamageReduction;
    }
}
