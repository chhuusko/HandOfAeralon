using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recall Memory", menuName = "Item/Card Data/Recall Memory", order = 1)]

public class RecallMemory : Card
{
    public override void PlayCard()
    {
        List<Card> cardsInDiscard = CardHandManager.GetInstance().GetDiscardPile();
        if (cardsInDiscard.Count > 0)
        {
            CardSelectHandler._instance.Setup(this, cardsInDiscard, 1);
        }
        
    }
    public override void CardSelect(Card selectedCard)
    {
        CardHandManager.GetInstance().AddCardFromDiscard(selectedCard);
    }
}
