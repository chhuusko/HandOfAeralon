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
    [SerializeField] private CardList _cardList;
    [SerializeField] private List<CardContainer> _cardsInHand;
    [SerializeField] private List<Card> _cardsInDeck;
    [SerializeField] private List<Card> _cardsInDiscardPile;
    [SerializeField] private int _maxHand = 3;
    
    private int _maxMana = 5;
    private int _mana = 0;

    public static Action<int> onManaChange;
    public static CardHandManager GetInstance() {return _instance;}
    public void ManaChanged(){ onManaChange?.Invoke(_mana); }
    private void Awake()
    {
        _instance = this;
        AddRandomCardsToDeck();
        drawHand();
    }
    private void AddRandomCardsToDeck()
    {
        for (int i = 0; i < 30; i++)
        {
            _cardsInDeck.Add(_cardList.GetRandomCard());
        }
    }
    public void drawHand()
    {
        _cardsInHand.RemoveAll(o => o == null);
        if (_cardsInDeck.Count == 0)
        {
            AddRandomCardsToDeck();
        }
        while (_maxHand > _cardsInHand.Count)
        {
            AddCardFromDeck();
        }
        
        AddSpaceing();
    }
    public void AddCardFromDeck()
    {
        CardContainer newCardContainer = Instantiate(_CardContainer, transform).GetComponent<CardContainer>();
        _cardsInHand.Add(newCardContainer);
        newCardContainer.AddCard(_cardsInDeck[0]);
        _cardsInDeck.RemoveAt(0);
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
    public void OpenDeck()
    {
        CardViewUI.GetInstance().UpdateCards(_cardsInDeck);
    }
    public void OpenDiscardPile()
    {
        CardViewUI.GetInstance().UpdateCards(_cardsInDiscardPile);
    }
    public void RemoveCard(CardContainer cardContainer)
    {
        _cardsInHand.Remove(cardContainer);
        Destroy(cardContainer.gameObject);
        _cardsInDiscardPile.Add(cardContainer.GetCard());
        drawHand();
    }
    public void ChangeMana(int change)
    {
        _mana += change;
        ManaChanged();
    }
    public void SetUIActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    public int GetMana()
    {
        return _mana;
    }
    public int GetMaxMana()
    {
        return _maxMana;
    }
    public List<Card> GetDeck()
    {
        return _cardsInDeck;
    }
    private List<Card> GetDiscardPile()
    {
        return _cardsInDiscardPile;
    }


}
