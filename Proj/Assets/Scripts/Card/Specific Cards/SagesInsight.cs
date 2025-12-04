using UnityEngine;
[CreateAssetMenu(fileName = "Sages Insight", menuName = "Item/Card Data/Sages Insight", order = 1)]
public class SagesInsight : Card
{
    public override void PlayCard()
    {
        for (int i = 0; i < 2; i++)
        {
            CardHandManager.GetInstance().AddCardFromDeck();
            //Debug.Log(CardHandManager.GetInstance().GetCardsInHand()[CardHandManager.GetInstance().GetCardsInHand().Count - 1].GetCard().cost);
            //CardHandManager.GetInstance().GetCardsInHand()[CardHandManager.GetInstance().GetCardsInHand().Count - 1].GetCard().cost -= 1;
        }
    }
}
