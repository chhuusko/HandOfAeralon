using FMODUnity;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardHandManager : MonoBehaviour
{
    //Controlls hand 
    private static CardHandManager _instance;

    [SerializeField] private GameObject _CardContainer;
    [SerializeField] private GameObject _zoomedCard;
    [SerializeField] private Transform _Hand;
    [SerializeField] private Transform _mulligan;
    [SerializeField] private CardList _cardList;
    [SerializeField] private List<CardContainer> _cardsInHand;
    [SerializeField] private List<Card> _cardsInDeck;
    [SerializeField] private List<Card> _cardsInDiscardPile;

    [SerializeField] private TextMeshProUGUI _deckText, _discardText;

    [SerializeField] private DeckPreset _deckPreset; 

    //sounds
    [SerializeField] private EventReference drawSound, hoverSound, playSound, deckShuffleSound, discardSound;


    // presets
    [SerializeField] private int turnsTillCard = 4;
    private int tempTurnsTillCard;
    private int _maxMana = 10;
    private int _mana = 5;
    private int _cardsPlayedThisTurn = 0;
    private static int _maxHand = 7;
    private static int beginningDraw = 5;

    // Onhover
    GameObject _addedZoomedCard;
    CardContainer _activeContainer;

    // turneffect
    public List<TurnEffect> turnEffects;

    //
    InputController _controller;

    //  bool
    bool isCombat;

    // view
    [SerializeField] CardViewUI cardView;
    [SerializeField] CardSelectViewUI cardSelect;

    // event
    public static Action<Card> onCardUse;
    public static Action<int> onManaChange;
    public static Action<Character> onTargetCharacter;
    public static Action<Character, Card> onCardTargetCharacter;
    public static Action<bool> onDrag;
    public static Action<bool> onHover;

    public static CardHandManager GetInstance() {return _instance;}
    public void ManaChanged(){ onManaChange?.Invoke(_mana); }
    public void Dragged(bool isDragEnter) { onDrag?.Invoke(isDragEnter); }
    public void Hovered(bool isHoverEnter) { onHover?.Invoke(isHoverEnter); }
    public void CardUsed(Card usedCard) 
    { 
        onCardUse?.Invoke(usedCard); 
        AudioManager.Instance.PlayOneShot(playSound, transform.position); 
    }
    public void CharacterTarget(Character targetCharacter) { onTargetCharacter?.Invoke(targetCharacter); }
    public void CardTargetCharacter(Card usedCard, Character target) { onCardTargetCharacter?.Invoke(target, usedCard); 
    }
    private void Awake()
    {
        _controller = new InputController();
        _instance = this;
        if (GlobalGameManager.GetInstance() != null)
        {
            _cardsInDeck = new List<Card>(GlobalGameManager.GetInstance().GetGameData().cardList);
        }
        else
        {
            _cardsInDeck = new List<Card>(_deckPreset.GetCards().Count);
            foreach (Card card in _deckPreset.GetCards())
            {
                Card clone = Instantiate(card);
                _cardsInDeck.Add(clone);
            }
        }
        drawHand();
        UpdatePileTexts();
    }

    private void OnEnable()
    {
        CombatEventManager.OnCombatTurnChange += TurnChanged;

        onCardTargetCharacter += TurnEffects;

        _controller.Enable();
        _controller.Developer.SkipLevel.performed += SkipLevel;
    }

    

    private void OnDisable()
    {
        CombatEventManager.OnCombatTurnChange -= TurnChanged;

        onCardTargetCharacter -= TurnEffects;

        _controller.Disable();
        _controller.Developer.SkipLevel.performed -= SkipLevel;
    }
    private void SkipLevel(InputAction.CallbackContext context)
    {
        LevelManager.GetInstance().StartNextLevel();
    }
    public void drawHand()
    {
        _cardsInHand.RemoveAll(o => o == null);
        while (beginningDraw > _cardsInHand.Count && _cardsInDeck.Count != 0)
        {
            
            if(_cardsInDeck.Count == 0)
            {
                _cardsInDeck = _cardsInDiscardPile;
            }
            AddCardFromDeck();
        }
        AddSpaceing();
    }
    public void AddCardFromDeck(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            AddCardFromDeck();
        }
        AddSpaceing();
    }
    public CardContainer AddCardFromDeck()
    {
        if (_cardsInDeck.Count == 0)
        {
            if(_cardsInDiscardPile.Count > 0)
            {
                //Add discard to draw pile
                AudioManager.Instance.PlayOneShot(deckShuffleSound, transform.position);
                _cardsInDeck = new List<Card>(_cardsInDiscardPile);
                _cardsInDiscardPile.Clear();
            }
            
        }

        if (_cardsInDeck.Count == 0) return null;
        if (_maxHand <= _cardsInHand.Count) return null;

        CardContainer newCardContainer = Instantiate(_CardContainer, _Hand).GetComponent<CardContainer>();
        _cardsInHand.Add(newCardContainer);
        int newCardIndex = UnityEngine.Random.Range(0, _cardsInDeck.Count);
        newCardContainer.AddCard(_cardsInDeck[newCardIndex]);
        _cardsInDeck.RemoveAt(newCardIndex);
        AddSpaceing();
        return newCardContainer;
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
        cardView.UpdateCards(_cardsInDeck);
        cardView.gameObject.SetActive(true);
    }
    public void OpenDiscardPile()
    {
        cardView.UpdateCards(_cardsInDiscardPile);
        cardView.gameObject.SetActive(true);
    }
    public void OpenCardSelect()
    {

    }
    public void RemoveCardFromHand(CardContainer cardContainer)
    {
        _cardsInHand.Remove(cardContainer);
        Destroy(cardContainer.gameObject);
        if (!cardContainer.GetCard().tags.Contains(CardTag.Etherial))
        {
            _cardsInDiscardPile.Add(cardContainer.GetCard());
        }
        AddSpaceing();
        _cardsPlayedThisTurn++;
        UpdatePileTexts();
        AudioManager.Instance.PlayOneShot(discardSound, transform.position);
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
                AddCardFromDeck();
            }
        }

        //handle etherial cards
        List<CardContainer> removeList = new List<CardContainer>();
        for (int i = 0; i < _cardsInHand.Count; i++)
        {
            if (_cardsInHand[i].GetCard().tags.Contains(CardTag.Etherial))
            {
                removeList.Add(_cardsInHand[i]);
            }
        }

        foreach (CardContainer card in removeList)
        {
            RemoveCardFromHand(card);
        }
        UpdatePileTexts();
        turnEffects.Clear();
    }
    public int GetCardsPlayedThisTurn()
    {
        return _cardsPlayedThisTurn;
    }
    public void ShowHighlightedCard(CardContainer container, Vector3 position)
    {
        if (_addedZoomedCard != null)
        {
            Destroy(_addedZoomedCard);
        }

        if (_activeContainer != null)
        {
            _activeContainer.setVisible(true);
        }

        _activeContainer = container;

        _addedZoomedCard = Instantiate(
            _zoomedCard,
            position,
            Quaternion.identity,
            CanvasManager.instance.OverlayCanvas.transform
        );
        _addedZoomedCard.GetComponent<CardUI>().SetUpUIElements(container.GetCard(), true);
        AudioManager.Instance.PlayOneShot(hoverSound, transform.position);
    }

    public void HideHighlightedCard()
    {
        if (_addedZoomedCard != null)
        {
            _addedZoomedCard.GetComponent<InfoPanelHandler>().SetShowInfoPanel(false);
            Destroy(_addedZoomedCard);
        }
    }
    public void AddCardToHand(Card newCard)
    {
        CardContainer newCardContainer = Instantiate(_CardContainer, _Hand).GetComponent<CardContainer>();
        _cardsInHand.Add(newCardContainer);
        newCardContainer.AddCard(newCard);

    }
    private void TurnEffects(Character character, Card card)
    {
        foreach(TurnEffect effect in turnEffects)
        {
            effect.Effect(character, card);
        }
    }
    public void OverrideManager()
    {

    }
    public void UpdatePileTexts()
    {
        _deckText.text = "Draw Pile (" + _cardsInDeck.Count + ")";
        _discardText.text = "Discard (" + _cardsInDiscardPile.Count + ")";
    }

}
