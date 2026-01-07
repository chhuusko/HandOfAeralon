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
    public CharacterData SelectedCharacter { get; private set; }
    
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
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _enemyActiveColor;
    [SerializeField] private Color _enemyInactiveColor;
    
    [Header("Misc")]
    [SerializeField] private Button _startCombatButton;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private GameObject _partyPanelText;
    public bool bCombatStarted { get; private set; }
    public Dictionary<CharacterData, PortraitButton> _characterPortraits = new();
    private List<PortraitButton> _portraitButtons = new();
    
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
        CombatEventManager.OnEnterCombatStateTakeTurn += DisableEndTurnButtonBorder;
        CombatEventManager.OnCharacterDeath += UpdateCharacterPortraits;
        CombatEventManager.OnCharacterPlaced += SetStartCombatButton;
        CombatEventManager.OnAbilityDataCreated += EnableEndTurnButtonBorder;
        CombatEventManager.OnCharacterMove += EnableEndTurnButtonBorder;

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
        if (!CombatGrid._instance.AllCharactersPlaced())
        {
            return;
        }
     
        _startCombatButton.interactable = true;
        _startCombatButton.transform.Find("Focus").gameObject.SetActive(true);
    }

    public void StartCombat()
    {
        if (!CombatGrid._instance.AllCharactersPlaced())
            return;

        OnStartCombatButtonPressed?.Invoke();
        
        _startCombatButton.interactable = false;
        _startCombatButton.gameObject.SetActive(false);
        _endTurnButton.gameObject.SetActive(true);
        _hand.SetActive(true);
        _placeCharactersPanel.SetActive(false);
        
        // Set abilities for first character.
        _abilityScript.LoadAbilities(Selector._instance.GetSelectedCharacter()?.Data);
    }

    private void SetSelectedCharacter(Character character)
    {
        SelectedCharacter = character.Data;
    }

    private void SetSelectedCharacter(PortraitButton portraitButton)
    {
        SetSelectedCharacter(portraitButton.Character);
    }

    private void EnableEndTurnButtonBorder(Character character, bool isMoving)
    {
        EnableEndTurnButtonBorder(character);
    }
    
    private void EnableEndTurnButtonBorder(AbilityExecutionData data)
    {
        if (!data.Caster)
        {
            return;
        }
        
        EnableEndTurnButtonBorder(data.Caster);
    }

    private void EnableEndTurnButtonBorder(Character character)
    {
        // Check if character can still act.
        if (character.GetFaction() == Faction.Enemy || character.CanUseAbility)
        {
            return;
        }
        
        // Rogues need to expend all movement points.
        if (character.GetCharacterClass() == CharacterClass.Rogue && character.GetMovementPoints() == 0)
        {
            SetEndTurnButtonBorder(true);
        }
        else if (!character.CanMove)
        {
            SetEndTurnButtonBorder(true);
        }
    }

    private void DisableEndTurnButtonBorder(Character character)
    {
        SetEndTurnButtonBorder(false);
    }

    private void SetEndTurnButtonBorder(bool active)
    {
        _endTurnButton.transform.Find("Border").gameObject.SetActive(active);
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
        // UpdateCharacterPortraits();
        UpdateActivePortrait(c);
        UpdatePortraitColors(c);
        
        SetSelectedCharacter(c);
        CurrentTurnCharacter = c;
        
        // _characterPortraitPanel.gameObject.SetActive(true);
        _deckButton.gameObject.SetActive(true);
        _turnOrder.SetActive(true);
        _activeCharacterBorder.gameObject.SetActive(true);
        _discardPileButton.gameObject.SetActive(true);
        _manaPanel.gameObject.SetActive(true);
        _startCombatButton.gameObject.SetActive(true);
        _cardHandManager.SetActive(true);
        _placeCharactersPanel.SetActive(true);
        _combatLogParent.SetActive(true);
        // _partyPanelText.SetActive(true);
        
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
        ClearPortraitColors();
    }

    private void SetEndTurnButtonInteractable()
    {
        if (CurrentTurnCharacter && CurrentTurnCharacter.GetFaction() == Faction.Friendly)
        {
            _endTurnButton.interactable = true;
        }
    }

    private void SetEndTurnButtonUninteractable()
    {
        _endTurnButton.interactable = false;
    }

    private void UpdateCharacterPortraits(Character character)
    {
        // UpdateCharacterPortraits();
    }
    
    /// <summary>
    /// Sets all character portraits in combat UI to reflect current party.
    /// </summary>
    // private void UpdateCharacterPortraits()
    // {
    //     GameData gameData = GlobalGameManager.GetInstance().GetGameData();
    //     List<CharacterData> heroList = gameData.heroDataList;
    //
    //     if (heroList == null)
    //     {
    //         DebugLog.JoppaLog("No HeroList");
    //         return;
    //     }
    //     
    //     ClearCharacterPortraits();
    //
    //     for (int i = 0; i < heroList.Count; i++)
    //     {
    //         CharacterData c = heroList[i];
    //         PortraitButton pb = CreateCharacterPortrait(CombatManager._instance.GetCharacterDataDict()[c], _portraitSlots[i]);
    //         pb.Button.image.color = _inactiveColor;
    //         _portraitButtons.Add(pb);
    //         _characterPortraits.TryAdd(pb.Character.Data, pb);
    //     }
    // }

    private void ClearCharacterPortraits()
    {
        _portraitButtons.Clear();
        
        _characterPortraits.Clear();

        for (int i = 0; i < _portraitSlots.Length; i++)
        {
            if (_portraitSlots[i].childCount > 0)
            {
                Destroy(_portraitSlots[i].GetChild(0).gameObject);
            }
        }
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
        
        button.image.sprite = c.GetClassData().classImage;

        if (c.GetFaction() == Faction.Enemy)
        {
            button.image.color = _enemyActiveColor;
        }
        
        PortraitButton pb = button.GetComponent<PortraitButton>();
        pb.Character = c;

        pb.OnClickPortraitButton += SetSelectedCharacter;
        pb.OnClickPortraitButton += UpdatePortraitColors;
        pb.OnClickPortraitButton += _abilityScript.LoadAbilities;
        
        return pb;
    }

    private void ClearPortraitColors()
    {
        foreach (var pb in _portraitButtons)
        {
            pb.GetComponent<Image>().color = _inactiveColor;
        }
    }
    
    private void UpdatePortraitColors(Character c) 
    {
        if (c == null || c.Data == null)
        {
            return;
        }

        if (!_characterPortraits.TryGetValue(c.Data, out var pb))
        {
            return;
        }
        
        UpdatePortraitColors(pb);
    }

    /// <summary>
    /// Updates colors for all current portraits to show faction and if the character is selected.
    /// </summary>
    /// <param name="selectedPortrait">The currently selected portrait.</param>
    private void UpdatePortraitColors(PortraitButton selectedPortrait)
    {
        bool friendly; 
        
        foreach (var pb in _portraitButtons)
        {
            friendly = pb.Character.GetFaction() == Faction.Friendly;
            pb.GetComponent<Image>().color = friendly ? _inactiveColor : _enemyInactiveColor;
        }

        if (!selectedPortrait)
        {
            return;
        }
        friendly = selectedPortrait.Character.GetFaction() == Faction.Friendly;
        selectedPortrait.GetComponent<Image>().color = friendly ? _activeColor : _enemyActiveColor;
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
        CombatEventManager.OnEnterCombatStateTakeTurn -= DisableEndTurnButtonBorder;
        CombatEventManager.OnCharacterDeath -= UpdateCharacterPortraits;
        CombatEventManager.OnCharacterPlaced -= SetStartCombatButton;
        CombatEventManager.OnAbilityDataCreated -= EnableEndTurnButtonBorder;
        CombatEventManager.OnCharacterMove -= EnableEndTurnButtonBorder;
        
        Selector._instance.OnCharacterSelected -= SetSelectedCharacter;
        Selector._instance.OnCharacterSelected -= _abilityScript.LoadAbilities;
        Selector._instance.OnCharacterSelected -= UpdatePortraitColors;
        Selector._instance.OnCharacterDeselected -= DeselectCharacter;
        Selector._instance.OnCharacterActionStarted -= SetEndTurnButtonUninteractable;
        Selector._instance.OnCharacterActionStopped -= SetEndTurnButtonInteractable;

        foreach (var pb in _portraitButtons)
        {
            pb.OnClickPortraitButton -= SetSelectedCharacter;
            pb.OnClickPortraitButton -= UpdatePortraitColors;
            pb.OnClickPortraitButton -= _abilityScript.LoadAbilities;
        }
    }
}
