using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Selective Purge", menuName = "Item /Card Data/SelectivePurge", order = 1)]
public class SelectivePurge : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void PlayCard()
    {

        List<Card> cardsInHand = new List<Card>();
        foreach (CardContainer card in CardHandManager.GetInstance().GetCardsInHand())
        {
            if (card.GetCard() != this)
            {
                cardsInHand.Add(card.GetCard());
            }
            
        }

        CardSelectHandler._instance.Setup(this, cardsInHand, 1);

    }
    public override void CardSelect(Card selectedCard)
    {
        CardHandManager cardHandManager = CardHandManager.GetInstance();
        List<CardContainer> cards = cardHandManager.GetCardsInHand();

        cardHandManager.AddCardFromDeck(2);

        foreach (CardContainer card in cards)
        {
            if (card.GetCard() == selectedCard)
            {
                CardHandManager.GetInstance().RemoveCardFromHand(card);
                return;
            }
        }
    }
}
