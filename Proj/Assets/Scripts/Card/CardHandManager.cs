using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CardHandManager : MonoBehaviour
{
    //Controlls hand 

    public static CardHandManager _instance;
    [SerializeField] private GameObject _CardContainer;
    [SerializeField] private List<CardContainer> _cardsInHand;
    [SerializeField] private int _maxHand = 3;
    [SerializeField] private CardList _cardList;
    private int _maxMana = 5;
    private int _mana = 0;

    public static Action<int> onManaChange;
    public static CardHandManager GetInstance() {return _instance;}
    public void ManaChanged(){ onManaChange?.Invoke(_mana); }
    private void Awake()
    {
        _instance = this;
        drawHand();
    }
    public void drawHand()
    {
        _cardsInHand.RemoveAll(o => o == null);
        while (_maxHand > _cardsInHand.Count)
        {
            AddCard();
        }
        AddSpaceing();
    }
    public void AddCard()
    {
        CardContainer newCardContainer = Instantiate(_CardContainer, transform).GetComponent<CardContainer>();
        _cardsInHand.Add(newCardContainer);
        newCardContainer.AddCard(_cardList.GetRandomCard());
        
    }

    public void AddSpaceing()
    {
        for (int i = 0; i < _cardsInHand.Count; i++)
        {
            Vector3 position = transform.position + new Vector3(-(150f * (_cardsInHand.Count - 1)) / 2f, 0, 0) + new Vector3(i * 150f, 0, 0);
            _cardsInHand[i].transform.position = position;
            _cardsInHand[i].SetPos(position);
        }
    }

    public void Mulligan()
    {
        if (_cardsInHand.Count > 1)
        {
            _maxHand--;
            foreach (CardContainer card in _cardsInHand)
            {
                Destroy(card.gameObject);
            }
            _cardsInHand.Clear();
            drawHand();
        }
    }
    public void RemoveCard(CardContainer cardContainer)
    {
        _cardsInHand.Remove(cardContainer);
        Destroy(cardContainer.gameObject);
        drawHand();
    }
    public void ChangeMana(int change)
    {
        _mana += change;
        ManaChanged();
    }
    public int GetMaxMana()
    {
        return _maxMana;
    }
    public void SetUIActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }


}
