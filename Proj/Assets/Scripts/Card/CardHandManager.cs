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
    [SerializeField] private int turnsTillCard = 4;
    private int tempTurnsTillCard;
    private int _maxMana = 10;
    private int _mana = 5;
    private int _cardsPlayedThisTurn = 0;

    public static Action<int> onManaChange;
    public static CardHandManager GetInstance() {return _instance;}
    public void ManaChanged(){ onManaChange?.Invoke(_mana); }
    private void Awake()
    {
        _instance = this;
        if (GlobalGameManager.GetInstance() != null)
        {
            _cardsInDeck = new List<Card>(GlobalGameManager.GetInstance().GetGameData().cardList);
        }
        else
        {
            _cardsInDeck = new List<Card>(_deckPreset.GetCards());
        }
        drawHand();
    }
    private void OnEnable()
    {
        CombatEventManager.OnCombatTurnChange += TurnChanged;
    }
    private void OnDisable()
    {
        CombatEventManager.OnCombatTurnChange -= TurnChanged;
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
        AddRandomCardFromDeck();
    }
    public void AddCardFromDeck(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            AddRandomCardFromDeck();
        }
        AddSpaceing();
    }
    public void AddRandomCardFromDeck()
    {
        if (_cardsInDeck.Count == 0)
        {
            if(_cardsInDiscardPile.Count > 0)
            {
                 _cardsInDeck = new List<Card>(_cardsInDiscardPile);
                _cardsInDiscardPile.Clear();
            }
            
        }
        if (_cardsInDeck.Count == 0)
        {
            return;
        }
        CardContainer newCardContainer = Instantiate(_CardContainer, _Hand).GetComponent<CardContainer>();
        _cardsInHand.Add(newCardContainer);
        int newCardIndex = UnityEngine.Random.Range(0, _cardsInDeck.Count);
        newCardContainer.AddCard(_cardsInDeck[newCardIndex]);
        _cardsInDeck.RemoveAt(newCardIndex);
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
        _cardsPlayedThisTurn++;
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
    public List<Card> GetDiscardPile()
    {
        return _cardsInDiscardPile;
    }
    public List<CardContainer> GetCardsInHand()
    {
        return _cardsInHand;
    }
    private void TurnChanged(CombatTurn t)
    {
        if(t == CombatTurn.PlayerTurn)
        {
            _cardsPlayedThisTurn = 0;

            tempTurnsTillCard--;
            if (tempTurnsTillCard <= 0)
            {
                tempTurnsTillCard = turnsTillCard;
                AddRandomCardFromDeck();
            }
        
        }
        
    }
    public int GetCardsPlayedThisTurn()
    {
        return _cardsPlayedThisTurn;
    }

}
