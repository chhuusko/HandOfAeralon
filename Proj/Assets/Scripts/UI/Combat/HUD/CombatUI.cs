using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public event Action OnStartCombatButtonPressed;
    public event Action OnEndTurnButtonPressed;
    public static CombatUI Instance;
    
    [Header("Abilities")]
    [SerializeField] private GameObject _abilityPanelParent;
    [SerializeField] private AbilityUI _abilityScript;
    
    [Header("Movement Points")]
    public GameObject _movementPointsPanel;
    
    [Header("Mana")]
    [SerializeField] private TextMeshProUGUI _mana;
    [SerializeField] private Image _manaFill;

    [Header("Characters")]
    [SerializeField] private GameObject _placeCharactersPanel;
    [SerializeField] private GameObject _activeCharacterBorder;
    [SerializeField] private Button _characterPortraitButtonPrefab;
    [SerializeField] private ActiveTurnCharacterButton _activeCharacterScript;
    public Character CurrentTurnCharacter { get; private set; }
    public Character SelectedCharacter { get; private set; }
    
    [Header("Party")]
    [SerializeField] private Image _characterPortraitPanel;
    [SerializeField] private Transform[] _portraitSlots;
    
    [Header("Turn Order")]
    [SerializeField] private GameObject _turnOrder;
    [SerializeField] private TurnOrder _turnOrderScript;
    
    [Header("Combat Log")]
    [SerializeField] private CombatLog _combatLog;
    [SerializeField] private GameObject _combatLogParent;

    [Header("Battle Counter")]
    [SerializeField] private Image _levelCounterPane;
    [SerializeField] private TextMeshProUGUI _levelCounter;

    [Header("Cards")]
    [SerializeField] private GameObject _hand;
    [SerializeField] private GameObject _cardHandManager;
    [SerializeField] private GameObject _deckButton;
    [SerializeField] private GameObject _discardPileButton;
    [SerializeField] private GameObject _manaPanel;
    
    [Header("Colors")]
    [SerializeField] private Color _portraitActiveColor;
    [SerializeField] private Color _portraitInactiveColor;
    [SerializeField] private Color _buttonInteractableColor;
    [SerializeField] private Color _buttonUninteractableColor;
    
    [Header("Buttons")]
    [SerializeField] private Button _startCombatButton;
    [SerializeField] private Image _startCombatButtonBorder;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Image _endTurnButtonBorder;
    
    [Header("Misc")]
    [SerializeField] private GameObject _partyPanelText;
    public bool bCombatStarted { get; private set; }
    public Dictionary<Character, PortraitButton> _characterPortraits = new();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        CardHandManager.onManaChange += UpdateManaText;
        
        CombatEventManager.OnEnterCombatStateLoadNextLevel += DisablePanels;
        CombatEventManager.OnEnterCombatStatePlaceCharacter += PlaceCharacterStarted;
        CombatEventManager.OnExitCombatStatePlaceCharacter += PlaceCharactersEnded;
        CombatEventManager.OnEnterCombatStateTakeTurn += StartTurn;
        CombatEventManager.OnEnterCombatStateTakeTurn += UpdateActivePortrait;
        CombatEventManager.OnEnterCombatStateTakeTurn += DisableEndTurnButtonFocus;
        CombatEventManager.OnCharacterPlaced += SetStartCombatButton;
        CombatEventManager.OnAbilityDataCreated += EnableEndTurnButtonFocus;
        CombatEventManager.OnCharacterMove += EnableEndTurnButtonFocus;

        StartCoroutine(WaitForSelector());
    }
    
    private IEnumerator WaitForSelector()
    {
        while (!Selector._instance)
        {
            yield return null;
        }
        
        Selector._instance.OnCharacterSelected += SetSelectedCharacter;
        Selector._instance.OnCharacterSelected += _abilityScript.LoadAbilities;
        Selector._instance.OnCharacterSelected += UpdatePortraitColors;
        Selector._instance.OnCharacterDeselected += DeselectCharacter;
        Selector._instance.OnCharacterActionStarted += SetEndTurnButtonUninteractable;
        Selector._instance.OnCharacterActionStopped += SetEndTurnButtonInteractable;
    }

    /// <summary>
    /// Enables the start combat button border once all characters have been placed.
    /// </summary>
    private void SetStartCombatButton()
    {
        if (!CombatGrid._instance.AllCharactersPlaced() || !_startCombatButton)
        {
            return;
        }
     
        _startCombatButton.interactable = true;
        _startCombatButton.transform.Find("Focus")?.gameObject.SetActive(true);
        _startCombatButtonBorder.color = _buttonInteractableColor;
    }

    public void StartCombat()
    {
        if (!CombatGrid._instance.AllCharactersPlaced())
        {
            return;
        }
        
        OnStartCombatButtonPressed?.Invoke();
        
        _startCombatButton.interactable = false;
        _startCombatButton.gameObject.SetActive(false);
        _endTurnButton.gameObject.SetActive(true);
        _hand.SetActive(true);
        _placeCharactersPanel.SetActive(false);
        
        // Set abilities for first character.
        _abilityScript.LoadAbilities(Selector._instance.GetSelectedCharacter());
    }

    private void SetSelectedCharacter(Character character)
    {
        SelectedCharacter = character;
    }

    public void EndTurn()
    {
        OnEndTurnButtonPressed?.Invoke();
    }

    public void ShowDeck()
    {
        CardHandManager.GetInstance().OpenDeck();
    }

    public void ShowDiscardPile()
    {
        CardHandManager.GetInstance().OpenDiscardPile();
    }

    /// <summary>
    /// Sets combat specific UI objects as active and calls methods to update them.
    /// </summary>
    private void PlaceCharacterStarted()
    {
        Character c = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        UpdateActivePortrait(c);
        UpdatePortraitColors(c);
        
        SetSelectedCharacter(c);
        CurrentTurnCharacter = c;
        
        _deckButton.gameObject.SetActive(true);
        _turnOrder.SetActive(true);
        _activeCharacterBorder.gameObject.SetActive(true);
        _discardPileButton.gameObject.SetActive(true);
        _manaPanel.gameObject.SetActive(true);
        _startCombatButton.gameObject.SetActive(true);
        _cardHandManager.SetActive(true);
        _placeCharactersPanel.SetActive(true);
        _combatLogParent.SetActive(true);
        
        // Enable the parent but set all child objects as hidden, so scripts can run, while not showing the element.
        _abilityPanelParent.SetActive(true);
        _abilityScript.SetPanelsActive(false);

        _levelCounter.text = $"Level {GlobalGameManager.GetInstance().GetTotalBattlesWon() + 1}";
        _levelCounter.gameObject.SetActive(true);
        _levelCounterPane.gameObject.SetActive(true);

        UpdateManaText(CardHandManager.GetInstance().GetMana());
    }

    private void DisablePanels()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    private void PlaceCharactersEnded()
    {
        bCombatStarted = true;
    }

    /// <summary>
    /// Calls corresponding methods to update UI elements at the start of turn.
    /// </summary>
    /// <param name="c">The character which's turn has started.</param>
    private void StartTurn(Character c)
    {
        UpdateActivePortrait(c);
        UpdatePortraitColors(c);
        
        SetSelectedCharacter(c);
        CurrentTurnCharacter = c;

        // Player can only end turn for friendly characters.
        if (c && c.GetFaction() == Faction.Enemy)
        {
            SetEndTurnButtonUninteractable();
        }
        else if (c && c.GetFaction() == Faction.Friendly)
        {
            SetEndTurnButtonInteractable();
        }
        
        _activeCharacterScript.SetCharacter(c);
        
        _abilityScript.LoadAbilities(SelectedCharacter);
    }

    private void DeselectCharacter()
    {
        SelectedCharacter = null;
        _abilityScript.ClearAbilityButtons();
    }
    
    private void EnableEndTurnButtonFocus(Character character, bool isMoving)
    {
        EnableEndTurnButtonFocus(character);
    }
    
    private void EnableEndTurnButtonFocus(AbilityExecutionData data)
    {
        if (!data.Caster)
        {
            return;
        }
        
        EnableEndTurnButtonFocus(data.Caster);
    }

    private void EnableEndTurnButtonFocus(Character character)
    {
        // Check if character can still act.
        if (character.GetFaction() == Faction.Enemy || character.CanUseAbility)
        {
            return;
        }
        
        // Rogues need to expend all movement points.
        if (character.GetCharacterClass() == CharacterClass.Rogue && character.GetMovementPoints() == 0)
        {
            SetEndTurnButtonFocus(true);
        }
        else if (!character.CanMove)
        {
            SetEndTurnButtonFocus(true);
        }
    }

    private void DisableEndTurnButtonFocus(Character character)
    {
        SetEndTurnButtonFocus(false);
    }

    private void SetEndTurnButtonFocus(bool active)
    {
        _endTurnButton?.transform.Find("Focus")?.gameObject.SetActive(active);
    }

    private void SetEndTurnButtonInteractable()
    {
        if (!_endTurnButton || !_endTurnButtonBorder ||
            !CurrentTurnCharacter || CurrentTurnCharacter.GetFaction() != Faction.Friendly)
        {
            return;
        }
        _endTurnButton.interactable = true;
        _endTurnButtonBorder.color = _buttonInteractableColor;
    }

    private void SetEndTurnButtonUninteractable()
    {
        if (!_endTurnButton || !_endTurnButtonBorder)
        {
            return;
        }
        
        _endTurnButton.interactable = false;
        _endTurnButtonBorder.color = _buttonUninteractableColor;
    }
    
    /// <summary>
    /// Creates a character portrait button.
    /// </summary>
    /// <param name="c">The character to create the portrait for.</param>
    /// <param name="parent">The parent transform the portrait is created under</param>
    /// <returns></returns>
    public PortraitButton CreateCharacterPortrait(Character c, Transform parent)
    {
        Button button = Instantiate(_characterPortraitButtonPrefab, parent);
        
        button.image.sprite = c.GetFaction() == Faction.Friendly ? 
            c.GetClassData().friendlyImage : c.GetClassData().enemyImage;
        
        PortraitButton pb = button.GetComponent<PortraitButton>();
        pb.Character = c;

        pb.OnClickPortraitButton += SetSelectedCharacter;
        pb.OnClickPortraitButton += UpdatePortraitColors;
        pb.OnClickPortraitButton += _abilityScript.LoadAbilities;
        
        return pb;
    }

    /// <summary>
    /// Updates colors for all current portraits to show if the character is selected.
    /// </summary>
    /// <param name="character">The currently selected character.</param>
    private void UpdatePortraitColors(Character character)
    {
        if (!character)
        {
            return;
        }
        _characterPortraits.TryGetValue(character, out var button);

        if (!button)
        {
            return;
        }
        button.GetComponent<Image>().color = _portraitActiveColor;
    }

    private void UpdateManaText(int mana)
    {
        _mana.text = mana.ToString();
        _manaFill.fillAmount = (float)mana / CardHandManager.GetInstance().GetMaxMana();
    }

    private void UpdateActivePortrait(Character c)
    {
        _activeCharacterScript.SetCharacter(c);
    }
    
    private void OnDisable()
    {
        CardHandManager.onManaChange -= UpdateManaText;
        
        CombatEventManager.OnEnterCombatStateLoadNextLevel -= DisablePanels;
        CombatEventManager.OnEnterCombatStatePlaceCharacter -= PlaceCharacterStarted;
        CombatEventManager.OnExitCombatStatePlaceCharacter -= PlaceCharactersEnded;
        CombatEventManager.OnEnterCombatStateTakeTurn -= StartTurn;
        CombatEventManager.OnEnterCombatStateTakeTurn -= UpdateActivePortrait;
        CombatEventManager.OnEnterCombatStateTakeTurn -= DisableEndTurnButtonFocus;
        CombatEventManager.OnCharacterPlaced -= SetStartCombatButton;
        CombatEventManager.OnAbilityDataCreated -= EnableEndTurnButtonFocus;
        CombatEventManager.OnCharacterMove -= EnableEndTurnButtonFocus;
        
        Selector._instance.OnCharacterSelected -= SetSelectedCharacter;
        Selector._instance.OnCharacterSelected -= _abilityScript.LoadAbilities;
        Selector._instance.OnCharacterSelected -= UpdatePortraitColors;
        Selector._instance.OnCharacterDeselected -= DeselectCharacter;
        Selector._instance.OnCharacterActionStarted -= SetEndTurnButtonUninteractable;
        Selector._instance.OnCharacterActionStopped -= SetEndTurnButtonInteractable;
    }
}
