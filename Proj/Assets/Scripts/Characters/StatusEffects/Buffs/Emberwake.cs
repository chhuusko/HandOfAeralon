using UnityEngine;

public class Emberwake : StatusEffect
{
    private bool _appliedThisTurn;
    
    public Emberwake(int duration = 3) : base(duration)
    {
    }

    public override void OnTurnStart()
    {
        _appliedThisTurn = false;
    }

    public override void OnBurnApplied()
    {
        var cards = CardHandManager.GetInstance().GetCardsInHand();

        var card = cards[UnityEngine.Random.Range(0, cards.Count)];
    }
}
