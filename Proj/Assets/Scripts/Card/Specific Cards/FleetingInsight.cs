using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fleeting Insight", menuName = "Item/Card Data/Fleeting Insight", order = 1)]
public class FleetingInsight : Card
{
    public override void PlayCard()
    {
        for (int i = 0; i < 3; i++)
        {
            List<Card> _unlockedCards = CardsUnlocked.GetInstance().GetUnlockedCards();
            {
                Card clone = Instantiate(_unlockedCards[Random.Range(0, _unlockedCards.Count - 1)]);
                clone.TempSetCost(0);
                clone.tags.Add(CardTag.Etherial);
                CardHandManager.GetInstance().AddCardToHand(clone);
                Debug.Log(clone);
            }
        }
    }
    
}
