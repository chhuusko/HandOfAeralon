using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mental Purge", menuName = "Item/Card Data/Mental Purge", order = 1)]
public class MentalPurge : Card
{
    public override void PlayCard()
    {
        CardHandManager cardHandManager = CardHandManager.GetInstance();
        List<CardContainer> cards = cardHandManager.GetCardsInHand();
        int count = cards.Count-1;

        if (count > 1)
        {
            foreach (CardContainer card in cards)
            {
                if (!card.GetCard().tags.Contains(CardTag.Etherial) || card.GetCard() != this)
                {

                    cardHandManager.GetDiscardPile().Add(card.GetCard());
                }
                Destroy(card.gameObject);
            }
            cards.Clear();
            cardHandManager.AddCardFromDeck(count);
        }
    }
}
