using UnityEngine;

public class Emberwake : StatusEffect
{
    private bool _effectApplied;
    
    public Emberwake(int duration = 3) : base(duration)
    {
    }

    public override void OnTurnStart()
    {
        _effectApplied = false;
    }

    public override void OnBurnApplied()
    {
        if (_effectApplied)
        {
            return;
        }
        
        var data = Data as ChanceModifyingData;

        if (!data)
        {
            return;
        }
        
        var cards = CardHandManager.GetInstance().GetCardsInHand();

        if (cards.Count == 0)
        {
            return;
        }
        
        var card = cards[UnityEngine.Random.Range(0, cards.Count)];

        if (!card)
        {
            return;
        }
        
        card.GetCard().TempModifyCost(-data.Modifier);
        _effectApplied = true;
    }

    public override void ModifyBurnApplicationChance(ref float chance)
    {
        var data = Data as ChanceModifyingData;

        if (!data)
        {
            return;
        }

        var modifier = 1f + data.ChancePercent / 100f;
        chance *= modifier;
    }
}
