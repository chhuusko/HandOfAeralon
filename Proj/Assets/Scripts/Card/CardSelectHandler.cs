using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectHandler : MonoBehaviour
{
    public static CardSelectHandler _instance { get; private set; }
    [SerializeField] private CardSelectViewUI _view;
    [SerializeField] private Button _button;
    private Card _cardUsed, _selectedCard;
    public int _amount { get; private set; } // might change

    private void Awake()
    {
        //_view = GetComponent<CardSelectViewUI>();
        _instance = this;
    }
    public void Setup(Card card, List<Card> cardList, int amount)
    {
        _cardUsed = card;
        _amount = amount;
        _view.UpdateCards(cardList, amount, "text");
        _view.gameObject.SetActive(true);
        _button.interactable = false;
        Debug.Log("updatesView");

    }
    public void SetSelectCard(Card selectedCard)
    {
        _selectedCard = selectedCard;
        if (_selectedCard != null)
        {
            _button.interactable = true;
        }
    }
    public void Exit()
    {
        _cardUsed.CardSelect(_selectedCard);
        _cardUsed = null;
        _selectedCard = null;
        _view.ExitUI();
    }
}
