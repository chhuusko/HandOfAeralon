using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPackViewUIShop : CardViewUI
{
    [SerializeField] private TextMeshProUGUI aboveText;
    public void UpdateText()
    {
        aboveText.text = "Select (<color=Yellow>" + (CardPackCard.maxSelect-CardPackCard.GetSelectedCount()) + "</color>) Cards to Keep";
    }
    public override void UpdateCards(List<Card> newCardList)
    {
        gameObject.SetActive(true);

        if (_cardListUI.Count > 0) { ClearUI(); }
        if (newCardList == null) { Debug.Log("Nothing"); }

        _cardContent.transform.position = basePosition;
        StartCoroutine(addCardsSequence(newCardList));
    }
    IEnumerator addCardsSequence(List<Card> newCardList)
    {
        for (int i = 0; i < newCardList.Count; i++)
        {
            _cardListUI.Add(Instantiate(_cardUI, _cardContent));
            _cardListUI[i].GetComponent<CardUI>().SetUpUIElements(newCardList[i]);
            _cardListUI[i].GetComponent<CardPackCard>().SetCard(newCardList[i]);
            yield return new WaitForSeconds(0.2f);
        }
        
    }
    
}
