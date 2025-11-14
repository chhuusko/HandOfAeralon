using System.Collections.Generic;
using UnityEngine;

public class CardViewUIShop : CardViewUI
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void UpdateCards(List<Card> newCardList)
    {
        gameObject.SetActive(true);

        if (_cardListUI.Count > 0) { ClearUI(); }
        if (newCardList == null) { Debug.Log("Nothing"); }

        for (int i = 0; i < newCardList.Count; i++)
        {
            _cardListUI.Add(Instantiate(_cardUI, _cardContent));
            _cardListUI[i].GetComponent<SellableCardUI>().SetCard(newCardList[i]);
            int layer = i / 5;
            Vector3 position = transform.position + new Vector3(-600f + (i % 5 * 300f), (layer - 1) * -400f, 0);
            _cardListUI[i].transform.position = position;
        }
    }
}
