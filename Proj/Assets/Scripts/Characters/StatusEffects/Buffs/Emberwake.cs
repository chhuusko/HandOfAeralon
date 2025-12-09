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
        var data = Data as ChanceModifyingData;

        if (!data)
        {
            return;
        }
        
        var cards = CardHandManager.GetInstance().GetCardsInHand();

        var card = cards[UnityEngine.Random.Range(0, cards.Count)];
        
        card.GetCard().TempModifyCost((int)-data.Modifier);
    }

    public override void ModifyBurnApplicationChance(ref float chance)
    {
        var data = Data as ChanceModifyingData;

        if (!data)
        {
            return;
        }

        chance *= data.Chance;
    }
}
