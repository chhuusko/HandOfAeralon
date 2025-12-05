using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public enum PanelType { Card, Ability }

    public event Action OnStartCombatButtonPressed;
    public event Action OnEndTurnButtonPressed;
    public static CombatUI Instance;
    
    [SerializeField] private Image _abilityPanel;
    [SerializeField] private Image _characterPortraitPanel;
    [SerializeField] private Image _activeCharacterPortrait;
    [SerializeField] private GameObject _activeCharacterBorder;
    
    [SerializeField] private Button _startCombatButton;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Button _abilityButtonPrefab;
    [SerializeField] private Button _characterPortraitButtonPrefab;
    
    [SerializeField] private GameObject _placeCharactersPanel;
    
    [SerializeField] private TextMeshProUGUI _mana;
    [SerializeField] private ScrollRect _turnOrderScrollBar;

    // Turn order.
    [SerializeField] private GameObject _turnOrderPanel;
    private Character _currentTurnCharacter;
    
    // Colors.
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _enemyColor;
    
    // Combat log.
    [SerializeField] private GameObject _combatLogEntryPrefab;
    [SerializeField] private GameObject _combatLogPanel;
    [SerializeField] private GameObject _combatLogScrollbar;
    [SerializeField] private GameObject _combatLogButton;
    [SerializeField] private Transform _combatLogViewPort;
    
    // Cards.
    [SerializeField] private GameObject _hand;
    [SerializeField] private GameObject _cardHandManager;
    [SerializeField] private GameObject _deckButton;
    [SerializeField] private GameObject _discardPileButton;
    [SerializeField] private GameObject _manaPanel;
    
    private CharacterData _selectedCharacter;
    private bool _bCombatStarted;
    private bool _bCombatLogEnabled;
    
    private List<PortraitButton> _portraitButtons = new();
    private List<AbilityButton> _abilityButtons = new();
    private List<AbilityExecutionData> _combatLogEntries = new();
    
    private Dictionary<CharacterData, PortraitButton> _characterPortraits = new();
    
    private void OnEnable()
    {
        CardHandManager.onManaChange += UpdateManaText;
        
        CombatEventManager.OnEnterCombatStatePlaceCharacter += PlaceCharacterStarted;
        CombatEventManager.OnEnterCombatStateLoadNextLevel += DisablePanels;
        CombatEventManager.OnEnterCombatStateTakeTurn += StartTurn;
        CombatEventManager.OnEnterCombatStateTakeTurn += UpdateActivePortrait;
        CombatEventManager.OnTurnOrderChanged += UpdateTurnOrder;
        CombatEventManager.OnExitCombatStatePlaceCharacter += PlaceCharactersEnded;
        CombatEventManager.OnAbilityDataCreated += AddCombatLogEntry;
        CombatEventManager.OnAbilityCast += UpdateAbilityColors;
        CombatEventManager.OnCharacterMove += CharacterMoving;

        StartCoroutine(WaitForSelector());
    }

    private void OnDisable()
    {
        CardHandManager.onManaChange -= UpdateManaText;
        
        CombatEventManager.OnEnterCombatStatePlaceCharacter -= PlaceCharacterStarted;
        CombatEventManager.OnEnterCombatStateLoadNextLevel -= DisablePanels;
        CombatEventManager.OnEnterCombatStateTakeTurn -= StartTurn;
        CombatEventManager.OnEnterCombatStateTakeTurn -= UpdateActivePortrait;
        CombatEventManager.OnTurnOrderChanged -= UpdateTurnOrder;
        CombatEventManager.OnExitCombatStatePlaceCharacter -= PlaceCharactersEnded;
        CombatEventManager.OnAbilityDataCreated -= AddCombatLogEntry;
        CombatEventManager.OnAbilityCast -= UpdateAbilityColors;
        CombatEventManager.OnCharacterMove -= CharacterMoving;
        
        Selector._instance.OnCharacterSelected -= SetSelectedCharacter;
        Selector._instance.OnCharacterSelected -= LoadAbilities;
        Selector._instance.OnCharacterSelected -= UpdatePortraitColors;
        Selector._instance.OnCharacterDeselected -= DeselectCharacter;

        foreach (var pb in _portraitButtons)
        {
            pb.OnClickPortraitButton -= SetSelectedCharacter;
            pb.OnClickPortraitButton -= UpdatePortraitColors;
            pb.OnClickPortraitButton -= UpdateActivePortrait;
            pb.OnClickPortraitButton -= LoadAbilities;
        }
    }

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
        
        // Player 1 portrait displayed as default when no character has been selected yet.
        // UpdateSelectedPortrait(GlobalGameManager.GetInstance().GetGameData().heroDataList[0]);
    }
    
    private IEnumerator WaitForSelector()
    {
        while (!Selector._instance)
        {
            yield return null;
        }
        
        Selector._instance.OnCharacterSelected += SetSelectedCharacter;
        Selector._instance.OnCharacterSelected += LoadAbilities;
        Selector._instance.OnCharacterSelected += UpdatePortraitColors;
        Selector._instance.OnCharacterDeselected += DeselectCharacter;
    }

    public void StartCombat()
    {
        OnStartCombatButtonPressed?.Invoke();
        
        _startCombatButton.gameObject.SetActive(false);
        _endTurnButton.gameObject.SetActive(true);
        _abilityPanel.gameObject.SetActive(true);
        _hand.SetActive(true);
        _placeCharactersPanel.SetActive(false);
        
        // Set abilities for first character.
        LoadAbilities(Selector._instance.GetSelectedCharacter()?.Data);
    }

    private void SetSelectedCharacter(Character character)
    {
        _selectedCharacter = character.Data;
    }

    private void SetSelectedCharacter(CharacterData character)
    {
        _selectedCharacter = character;
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

    public void SetCombatLogActive()
    {
        _bCombatLogEnabled = !_bCombatLogEnabled;
        _combatLogPanel.SetActive(_bCombatLogEnabled);
        _combatLogScrollbar.SetActive(_bCombatLogEnabled);
    }

    private void AddCombatLogEntry(AbilityExecutionData data)
    {
        _combatLogEntries.Add(data);
        var go = Instantiate(_combatLogEntryPrefab, _combatLogViewPort);
        
        go.transform.Find("Icon").GetComponent<Image>().sprite = data.Ability.GetIcon();
        go.transform.Find("Text").GetComponent<TMP_Text>().text =
            $"{data.Caster.Data.ClassData.name} does {data.Damage} damage to {data.Target.Data.ClassData.name}";
    }

    private void PlaceCharacterStarted()
    {
        CharacterData c = GlobalGameManager.GetInstance().GetGameData().heroDataList[0];
        UpdateActivePortrait(c);
        UpdatePortraitColors(_characterPortraits[c]);
        LoadAbilities(c);
        StartCoroutine(ScrollToBottom());
        
        SetSelectedCharacter(c);
        _currentTurnCharacter = CombatManager._instance.GetCharacterDataDict()[_selectedCharacter];
        
        _characterPortraitPanel.gameObject.SetActive(true);
        _deckButton.gameObject.SetActive(true);
        _turnOrderPanel.gameObject.SetActive(true);
        _turnOrderScrollBar.gameObject.SetActive(true);
        _activeCharacterBorder.gameObject.SetActive(true);
        _discardPileButton.gameObject.SetActive(true);
        _manaPanel.gameObject.SetActive(true);
        _startCombatButton.gameObject.SetActive(true);
        _cardHandManager.SetActive(true);
        _combatLogButton.SetActive(true);
        
        UpdateCharacterPortraits();
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
        _bCombatStarted = true;
    }

    private void StartTurn(Character c)
    {
        UpdateActivePortrait(c);
        UpdatePortraitColors(c);
        
        SetSelectedCharacter(c.Data);
        _currentTurnCharacter = c;
        
        LoadAbilities(_selectedCharacter);
    }

    private void DeselectCharacter()
    {
        _selectedCharacter = null;
        
        ClearAbilityButtons();

        ClearPortraitColors();
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
        
        foreach (CharacterData c in heroList)
        {
            PortraitButton pb = CreateCharacterPortrait(c, _characterPortraitPanel.transform);
            pb.Button.image.color = _inactiveColor;
            _portraitButtons.Add(pb);
            _characterPortraits.TryAdd(pb.Character, pb);
        }
    }

    private void ClearCharacterPortraits()
    {
        _portraitButtons.Clear();
        
        // TODO: Clearing character portraits here will cause turn order to break most likely. Need to fix.
        _characterPortraits.Clear();

        for (int i = 0; i < _characterPortraitPanel.transform.childCount; i++)
        {
            Destroy(_characterPortraitPanel.transform.GetChild(i).gameObject);
        }
    }

    private PortraitButton CreateCharacterPortrait(CharacterData c, Transform parent)
    {
        Button button = Instantiate(_characterPortraitButtonPrefab, parent);
        
        button.image.sprite = c.ClassData.classImage;

        if (c.Faction == Faction.Enemy)
        {
            button.image.color = _enemyColor;
        }
        
        PortraitButton pb = button.GetComponent<PortraitButton>();
        pb.Character = c;

        pb.OnClickPortraitButton += SetSelectedCharacter;
        pb.OnClickPortraitButton += UpdatePortraitColors;
        pb.OnClickPortraitButton += UpdateActivePortrait;
        pb.OnClickPortraitButton += LoadAbilities;
        
        return pb;
    }
    
    private void UpdateTurnOrder(IReadOnlyList<Character> characters)
    {
        // Clear previous portraits.
        for (int i = 0; i < _turnOrderPanel.transform.childCount; i++)
        {
            Destroy(_turnOrderPanel.transform.GetChild(i).gameObject);
        }
        
        // Create portraits for current turn order.
        foreach (Character c in characters)
        {
            PortraitButton pb = CreateCharacterPortrait(c.Data, _turnOrderPanel.transform);
            _characterPortraits.TryAdd(pb.Character, pb);
        }
        
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null;
        
        // Set scroll to bottom.
        _turnOrderScrollBar.verticalNormalizedPosition = 0;
    }

    private void ClearPortraitColors()
    {
        foreach (var pb in _portraitButtons)
        {
            pb.GetComponent<Image>().color = _inactiveColor;
        }
    }

    private void UpdatePortraitColors(CharacterData c)
    {
        UpdatePortraitColors(CombatManager._instance.GetCharacterDataDict()[c]);
    }
    
    private void UpdatePortraitColors(Character c) 
    {
        UpdatePortraitColors(_characterPortraits[c.Data]);
    }

    private void UpdatePortraitColors(PortraitButton selectedPortrait)
    {
        foreach (var pb in _portraitButtons)
        {
            pb.GetComponent<Image>().color = _inactiveColor;
        }

        if (selectedPortrait)
        {
            selectedPortrait.GetComponent<Image>().color = _activeColor;
        }
    }

    private void UpdateManaText(int mana)
    {
        _mana.text = $"Mana\n{mana}/{CardHandManager.GetInstance().GetMaxMana()}";
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

    public void UpdateActivePortrait(Character c)
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
        }
        
        if (c.Faction == Faction.Enemy)
        {
            DebugLog.JoppaLog("Enemy");
            return;
        }
        
        _activeCharacterPortrait.gameObject.SetActive(true);
        _activeCharacterPortrait.sprite = c.ClassData.classImage;
    }
    
    public void SetCardsActive(bool active)
    {
        CardHandManager.GetInstance().SetUIActive(active);
        _abilityPanel.color = active ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }

    private void ClearAbilityButtons()
    {
        _abilityButtons.Clear();
        // Remove all current ability buttons.
        for (int i = 0; i < _abilityPanel.transform.childCount; i++)
        {
            Destroy(_abilityPanel.transform.GetChild(i).gameObject);
        }
        
        _abilityPanel.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Displays each available ability for the selected character.
    /// </summary>
    /// <param name="portraitButton">The character of which's abilities to display.</param>
    private void LoadAbilities(PortraitButton portraitButton)
    {
        LoadAbilities(portraitButton.Character);
    }

    private void LoadAbilities(Character c)
    {
        LoadAbilities(c.Data);
    }
    
    private void LoadAbilities(CharacterData character)
    {
        StartCoroutine(LoadAbilitiesNextFrame(character));
    }

    private IEnumerator LoadAbilitiesNextFrame(CharacterData character)
    {
        yield return null;
        
        if (character == null)
        {
            DebugLog.JoppaLog("No selected character");
            yield break;
        }

        // Don't show abilities for enemies.
        if (character.Faction == Faction.Enemy)
        {
            yield break;
        }

        if (!_bCombatStarted)
        {
            DebugLog.JoppaLog("Combat not started");
            yield break;
        }

        ClearAbilityButtons();
        
        _abilityPanel.gameObject.SetActive(true);

        for (int i = 0; i < character.Abilities.Count; i++)
        {
            var buttonGO = Instantiate(_abilityButtonPrefab.gameObject);
            buttonGO.SetActive(false);
            buttonGO.transform.SetParent(_abilityPanel.transform, false);
            
            var button = buttonGO.GetComponent<Button>();
            
            var ability = character.Abilities[i];
            button.image.sprite = ability.GetIcon();
            button.GetComponent<AbilityButton>().Ability = ability;
            
            var abilityButton = button.GetComponent<AbilityButton>();
            _abilityButtons.Add(abilityButton);
            
            UpdateAbilityColors(CombatManager._instance.GetCharacterDataDict()[character], abilityButton);
            
            buttonGO.SetActive(true);
        }
    }

    private void UpdateAbilityColors()
    {
        foreach (var abilityButton in _abilityButtons)
        {
            UpdateAbilityColors(CombatManager._instance.GetCharacterDataDict()[_selectedCharacter], abilityButton);
        }
    }

    private void UpdateAbilityColors(Character c, AbilityButton abilityButton)
    {
        bool interactable = false;

        DebugLog.JoppaLog($"c == _currentTurnCharacter: {c == _currentTurnCharacter}");
        DebugLog.JoppaLog($"_selectedCharacter.Faction: {_selectedCharacter.Faction == Faction.Friendly}");
        DebugLog.JoppaLog($"IsAbilityCooldownActive: {!c.IsAbilityCooldownActive(abilityButton.Ability)}");
        DebugLog.JoppaLog($"CanAttack: {c.CanUseAbility}");
        
        if (_bCombatStarted && c && _currentTurnCharacter && _selectedCharacter != null)
        {
            interactable = c == _currentTurnCharacter &&
                           _selectedCharacter.Faction == Faction.Friendly &&
                           !c.IsAbilityCooldownActive(abilityButton.Ability) &&
                           c.CanUseAbility && c.Data.ActiveAbilities.Contains(abilityButton.Ability);
        }

        abilityButton.Button.interactable = interactable;
    }

    private void CharacterMoving(bool moving)
    {
        foreach (var abilityButton in _abilityButtons)
        {
            abilityButton.Button.interactable = !moving;
        }
        
        _endTurnButton.interactable = !moving;
    }
}
