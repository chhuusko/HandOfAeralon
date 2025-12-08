using UnityEngine;

public class ManaConduction : Trait
{
    private int _manaUsed;

    public override void OnTurnStart()
    {
        _manaUsed = 0;
    }

    public override void OnCardPlayed(Card card)
    {
        _manaUsed += card.Getcost();
    }

    public override void ModifyOutgoingDamage(ref float damage, Ability ability)
    {
        if (ability is not ArcaneBolt_SingleTarget)
        {
            return;
        }
        
        var data = Data as DamageModifyingData;

        if (!data)
        {
            return;
        }

        damage *= (_manaUsed * data.DamageModifier);
    }
}
