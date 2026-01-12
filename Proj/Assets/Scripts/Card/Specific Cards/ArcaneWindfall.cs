using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Arcane Windfall", menuName = "Item/Card Data/Arcane Windfall", order = 1)]
public class ArcaneWindfall : Card
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void AfterCardPlay()
    {
        base.AfterCardPlay();
        for (int i = 0; i < 3; i++)
        {
            List<Card> _unlockedCards = CardsUnlocked.GetInstance().GetCardsByRarity(CardRarity.Uncommon);
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
