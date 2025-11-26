using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mental Purge", menuName = "Item/Card Data/Mental Purge", order = 1)]
public class MentalPurge : Card
{
    public override void PlayCard()
    {
        CardHandManager cardHandManager = CardHandManager.GetInstance();
        List<CardContainer> cards = cardHandManager.GetCardsInHand();
        int count = cards.Count;

        foreach (CardContainer card in cards)
        {
            cardHandManager.RemoveCard(card);
        }
        cardHandManager.AddCardFromDeck(count);
    }
}
