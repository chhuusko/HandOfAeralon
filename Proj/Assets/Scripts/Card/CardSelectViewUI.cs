using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSelectViewUI : CardViewUI
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI aboveText;
    private int amount;
    public void UpdateText(string text)
    {
        aboveText.text = "Select (<color=Yellow>" + (amount - CardSelect.GetSelectedCount()) + "</color>) Cards";
    }
    public void UpdateCards(List<Card> newCardList, int amount, string text)
    {
        gameObject.SetActive(true);

        if (_cardListUI.Count > 0) { ClearUI(); }
        if (newCardList == null) { Debug.Log("Nothing"); }
        UpdateText(text);
        _cardContent.transform.position = basePosition;
        gameObject.SetActive(true);

        for (int i = 0; i < newCardList.Count; i++)
        {
            _cardListUI.Add(Instantiate(_cardUI, _cardContent));
            _cardListUI[i].GetComponent<CardUI>().SetUpUIElements(newCardList[i]);
            _cardListUI[i].GetComponent<CardSelect>().SetCard(newCardList[i]);
            
        }
    }
}
