using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Arcane Echoform", menuName = "Item /Card Data/Arcane Echoform", order = 1)]
public class ArcaneEchoform : Card
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
        if (cardsInHand.Count > 0)
        {
            CardSelectHandler._instance.Setup(this, cardsInHand, 1);
        }
    }
    public override void CardSelect(Card selectedCard)
    {
        CardHandManager cardHandManager = CardHandManager.GetInstance();
        Card newCard = Instantiate(selectedCard);
        newCard.TempSetCost(selectedCard.GetCost());
        newCard.tags.Add(CardTag.Exhaust);
        cardHandManager.AddCardToHand(newCard);

    }
}
