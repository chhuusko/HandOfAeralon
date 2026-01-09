using UnityEngine;
[CreateAssetMenu(fileName = "Sages Insight", menuName = "Item/Card Data/Sages Insight", order = 1)]
public class SagesInsight : Card
{
    private CardContainer newCard;
    public override void AfterCardPlay()
    {
        base.AfterCardPlay();
        for (int i = 0; i < 2; i++)
        {
            if (newCard = CardHandManager.GetInstance().AddCardFromDeck())
            {
                if (newCard != null)
                {
                    Debug.Log(newCard.GetCard());
                    newCard.GetCard().TempModifyCost(-1);
                    newCard.UppdateCardUI();
                }
            }
            newCard = null;

        }
    }
}
