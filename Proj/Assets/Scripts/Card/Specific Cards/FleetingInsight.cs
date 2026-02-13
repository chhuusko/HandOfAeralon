using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fleeting Insight", menuName = "Item/Card Data/Fleeting Insight", order = 1)]
public class FleetingInsight : Card
{
    public override void AfterCardPlay()
    {
        base.AfterCardPlay();
        for (int i = 0; i < 3; i++)
        {
            List<Card> _unlockedCards = CardsUnlocked.GetInstance().GetCardsByRarity(CardRarity.Common);
            {
                Card clone = Instantiate(_unlockedCards[Random.Range(0, _unlockedCards.Count - 1)]);
                clone.TempSetCost(0);
                clone.tags.Add(CardTag.Ephemeral);
                CardHandManager.GetInstance().AddCardToHand(clone);
            }
        }
    }
    
}
