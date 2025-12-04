using UnityEngine;
[CreateAssetMenu(fileName = "Sages Insight", menuName = "Item/Card Data/Sages Insight", order = 1)]
public class SagesInsight : Card
{
    public override void PlayCard()
    {
        for (int i = 0; i < 2; i++)
        {
            CardContainer newCard = CardHandManager.GetInstance().AddCardFromDeck();
            if (newCard != null)
            {
                Debug.Log(newCard.GetCard());
                newCard.GetCard().cost -= 1;
                newCard.UppdateCardUI();
            }
        }
    }
}
