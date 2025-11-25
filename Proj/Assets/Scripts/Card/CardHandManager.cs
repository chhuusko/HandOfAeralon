using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CardHandManager : MonoBehaviour
{
    //Controlls hand 

    private static CardHandManager _instance;

    [SerializeField] private GameObject _CardContainer;
    [SerializeField] private Transform _Hand;
    [SerializeField] private Transform _mulligan;
    [SerializeField] private CardList _cardList;
    [SerializeField] private List<CardContainer> _cardsInHand;
    [SerializeField] private List<Card> _cardsInDeck;
    [SerializeField] private List<Card> _cardsInDiscardPile;
    [SerializeField] private int _maxHand = 3;
    
    [SerializeField] private DeckPreset _deckPreset; /// TEMP DECK
    private int _maxMana = 5;
    private int _mana = 5;

    public static Action<int> onManaChange;
    public static CardHandManager GetInstance() {return _instance;}
    public void ManaChanged(){ onManaChange?.Invoke(_mana); }
    private void Awake()
    {
        _instance = this;
        if (GlobalGameManager.GetInstance() != null)
        {
            _cardsInDeck = GlobalGameManager.GetInstance().GetGameData().cardList;
        }
        else
        {
            _cardsInDeck = new List<Card>(_deckPreset.GetCards());
        }
        drawHand();
    }
    public void drawHand()
    {
        _cardsInHand.RemoveAll(o => o == null);
        while (_maxHand > _cardsInHand.Count)
        {
            if(_cardsInDeck.Count == 0)
            {
                _cardsInDeck = _cardsInDiscardPile;
            }
            AddCardFromDeck();
        }
        AddSpaceing();
    }
    public void AddCardFromDeck()
    {
        CardContainer newCardContainer = Instantiate(_CardContainer, _Hand).GetComponent<CardContainer>();
        _cardsInHand.Add(newCardContainer);
        newCardContainer.AddCard(_cardsInDeck[0]);
        _cardsInDeck.RemoveAt(0);
        AddSpaceing();
    }
    public void AddCardFromDeck(int amount)
    {
        for(int i = 0; i < _cardsInHand.Count; i++)
        {
            CardContainer newCardContainer = Instantiate(_CardContainer, _Hand).GetComponent<CardContainer>();
            _cardsInHand.Add(newCardContainer);
            newCardContainer.AddCard(_cardsInDeck[0]);
            _cardsInDeck.RemoveAt(0);
        }
        AddSpaceing();
    }

    public void AddSpaceing()
    {
        for (int i = 0; i < _cardsInHand.Count; i++)
        {
            Vector3 position = _Hand.position + new Vector3(-(150f * (_cardsInHand.Count - 1)) / 2f, 0, 0) + new Vector3(i * 150f, 0, 0);
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
        AddSpaceing();
    }
    public void ChangeMana(int change)
    {
        if(_mana +  change > _maxMana)
            _mana = _maxMana;
        else
            _mana += change;

        ManaChanged();
    }
    public void SetUIActive(bool isActive)
    {
        // _Hand.gameObject.SetActive(isActive);
        // _mulligan.gameObject.SetActive(isActive);
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
    public List<CardContainer> GetCardsInHand()
    {
        return _cardsInHand;
    }


}
