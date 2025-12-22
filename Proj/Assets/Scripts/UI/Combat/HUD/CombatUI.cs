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
    
    [Header("Mana")]
    [SerializeField] private TextMeshProUGUI _mana;
    [SerializeField] private Image _manaFill;

    [Header("Characters")]
    [SerializeField] private GameObject _placeCharactersPanel;
    [SerializeField] private Image _activeCharacterPortrait;
    [SerializeField] private GameObject _activeCharacterBorder;
    [SerializeField] private Button _characterPortraitButtonPrefab;
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
        
        CombatEventManager.OnEnterCombatStatePlaceCharacter += PlaceCharacterStarted;
        CombatEventManager.OnEnterCombatStateLoadNextLevel += DisablePanels;
        CombatEventManager.OnEnterCombatStateTakeTurn += StartTurn;
        CombatEventManager.OnEnterCombatStateTakeTurn += UpdateActivePortrait;
        CombatEventManager.OnExitCombatStatePlaceCharacter += PlaceCharactersEnded;
        CombatEventManager.OnCharacterDeath += UpdateCharacterPortraits;

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

    public void StartCombat()
    {
        if (!CombatGrid._instance.AllCharactersPlaced())
            return;

        OnStartCombatButtonPressed?.Invoke();
        
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

    private void PlaceCharacterStarted()
    {
        Character c = CombatManager._instance.GetCombatTurnOrder().GetActiveCharacter();
        UpdateCharacterPortraits();
        UpdateActivePortrait(c);
        UpdatePortraitColors(c);
        
        SetSelectedCharacter(c);
        CurrentTurnCharacter = c;
        
        _characterPortraitPanel.gameObject.SetActive(true);
        _deckButton.gameObject.SetActive(true);
        _turnOrder.SetActive(true);
        _activeCharacterBorder.gameObject.SetActive(true);
        _discardPileButton.gameObject.SetActive(true);
        _manaPanel.gameObject.SetActive(true);
        _startCombatButton.gameObject.SetActive(true);
        _cardHandManager.SetActive(true);
        _placeCharactersPanel.SetActive(true);
        _combatLogParent.SetActive(true);
        _partyPanelText.SetActive(true);
        
        _abilityPanelParent.SetActive(true);

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

    private void StartTurn(Character c)
    {
        UpdateActivePortrait(c);
        UpdatePortraitColors(c);
        
        SetSelectedCharacter(c);
        CurrentTurnCharacter = c;
        _activeCharacterPortrait.GetComponent<PortraitButton>().Character = c;
        
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
        _endTurnButton.interactable = true;
    }

    private void SetEndTurnButtonUninteractable()
    {
        _endTurnButton.interactable = false;
    }

    private void UpdateCharacterPortraits(Character character)
    {
        UpdateCharacterPortraits();
    }
    
    /// <summary>
    /// Sets all character portraits in combat UI to reflect current party.
    /// </summary>
    private void UpdateCharacterPortraits()
    {
        GameData gameData = GlobalGameManager.GetInstance().GetGameData();
        List<CharacterData> heroList = gameData.heroDataList;

        if (heroList == null)
        {
            DebugLog.JoppaLog("No HeroList");
            return;
        }
        
        ClearCharacterPortraits();

        for (int i = 0; i < heroList.Count; i++)
        {
            CharacterData c = heroList[i];
            PortraitButton pb = CreateCharacterPortrait(CombatManager._instance.GetCharacterDataDict()[c], _portraitSlots[i]);
            pb.Button.image.color = _inactiveColor;
            _portraitButtons.Add(pb);
            _characterPortraits.TryAdd(pb.Character.Data, pb);
        }
    }

    private void ClearCharacterPortraits()
    {
        _portraitButtons.Clear();
        
        // TODO: Clearing character portraits here will cause turn order to break most likely. Need to fix.
        _characterPortraits.Clear();

        for (int i = 0; i < _portraitSlots.Length; i++)
        {
            if (_portraitSlots[i].childCount > 0)
            {
                Destroy(_portraitSlots[i].GetChild(0).gameObject);
            }
        }
    }

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
        pb.OnClickPortraitButton += UpdateActivePortrait;
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

    private void UpdatePortraitColors(PortraitButton selectedPortrait)
    {
        bool friendly; 
        
        foreach (var pb in _portraitButtons)
        {
            friendly = pb.Character.GetFaction() == Faction.Friendly;
            pb.GetComponent<Image>().color = friendly ? _inactiveColor : _enemyInactiveColor;
        }

        if (selectedPortrait)
        {
            friendly = selectedPortrait.Character.GetFaction() == Faction.Friendly;
            selectedPortrait.GetComponent<Image>().color = friendly ? _activeColor : _enemyActiveColor;
        }
    }

    private void UpdateManaText(int mana)
    {
        _mana.text = mana.ToString();
        _manaFill.fillAmount = (float)mana / CardHandManager.GetInstance().GetMaxMana();
    }

    private void ClearActivePortrait()
    {
        _activeCharacterPortrait.sprite = null;
        _activeCharacterPortrait.gameObject.SetActive(false);
    }
    
    private void UpdateActivePortrait(PortraitButton pb)
    {
        // UpdateActivePortrait(pb.Character);
    }

    private void UpdateActivePortrait(Character c)
    {
        if (!c)
        {
            ClearActivePortrait();
            DebugLog.JoppaLog("Null character");
            return;
        }
        UpdateActivePortrait(c.Data);
    }

    private void UpdateActivePortrait(CharacterData c)
    {
        if (c == null)
        {
            ClearActivePortrait();
            return;
        }
        
        _activeCharacterPortrait.gameObject.SetActive(true);
        _activeCharacterPortrait.sprite = c.ClassData.classImage;
        _activeCharacterPortrait.color = c.Faction == Faction.Friendly ? _activeColor : _enemyActiveColor;
    }
    
    private void OnDisable()
    {
        CardHandManager.onManaChange -= UpdateManaText;
        
        CombatEventManager.OnEnterCombatStatePlaceCharacter -= PlaceCharacterStarted;
        CombatEventManager.OnEnterCombatStateLoadNextLevel -= DisablePanels;
        CombatEventManager.OnEnterCombatStateTakeTurn -= StartTurn;
        CombatEventManager.OnEnterCombatStateTakeTurn -= UpdateActivePortrait;
        CombatEventManager.OnExitCombatStatePlaceCharacter -= PlaceCharactersEnded;
        CombatEventManager.OnCharacterDeath -= UpdateCharacterPortraits;
        
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
            pb.OnClickPortraitButton -= UpdateActivePortrait;
            pb.OnClickPortraitButton -= _abilityScript.LoadAbilities;
        }
    }
}
