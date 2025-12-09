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
    
    [SerializeField] private Image _abilityPanel;
    [SerializeField] private GameObject _abilityPanelParent;
    [SerializeField] private Image _characterPortraitPanel;
    [SerializeField] private Image _activeCharacterPortrait;
    [SerializeField] private GameObject _activeCharacterBorder;
    
    [SerializeField] private Button _startCombatButton;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Button _abilityButtonPrefab;
    [SerializeField] private Button _characterPortraitButtonPrefab;
    
    [SerializeField] private GameObject _placeCharactersPanel;
    
    [SerializeField] private ScrollRect _turnOrderScrollBar;
    
    // Mana.
    [SerializeField] private TextMeshProUGUI _mana;
    [SerializeField] private Image _manaFill;

    // Turn order.
    [SerializeField] private GameObject _turnOrderPanel;
    private Character _currentTurnCharacter;
    
    // Colors.
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _enemyActiveColor;
    [SerializeField] private Color _enemyInactiveColor;
    
    // Combat log.
    [SerializeField] private GameObject _combatLogEntryPrefab;
    [SerializeField] private GameObject _combatLogPanel;
    [SerializeField] private GameObject _combatLogScrollbar;
    [SerializeField] private GameObject _combatLogButton;
    [SerializeField] private Transform _combatLogViewPort;
    [SerializeField] private ScrollRect _combatLogScrollRect;
    
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
        Selector._instance.OnCharacterActionStarted -= SetEndTurnButtonUninteractable;
        Selector._instance.OnCharacterActionStopped -= SetEndTurnButtonInteractable;

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
        Selector._instance.OnCharacterActionStarted += SetEndTurnButtonUninteractable;
        Selector._instance.OnCharacterActionStopped += SetEndTurnButtonInteractable;
    }

    public void StartCombat()
    {
        OnStartCombatButtonPressed?.Invoke();
        
        _startCombatButton.gameObject.SetActive(false);
        _endTurnButton.gameObject.SetActive(true);
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
        // go.transform.Find("Text").GetComponent<TMP_Text>().text =
        //     $"{data.Caster.Data.ClassData.name} does {data.Damage} damage to {data.Target.Data.ClassData.name}";

        if (!data.Ability || !data.Target || !data.Caster)
        {
            return;
        }

        string text;
        
        // Check for type of ability.
        if (data.Ability.GetAbilityType() is Ability.Type.Elemental or Ability.Type.Physical)
        {
            text = $"{data.Caster.Data.ClassData.name} used {data.Ability.GetAbilityName()} and dealt " +
                   $"{data.Damage} damage to{(data.Target.GetFaction() == Faction.Friendly ? " " : " enemy")}" +
                   $" {data.Target.Data.ClassData.name}";
        }
        else
        {
            text = $"{data.Caster.Data.ClassData.name} used {data.Ability.GetAbilityName()}" +
                   $" on{(data.Target.GetFaction() == Faction.Friendly ? " " : " enemy")} " +
                   $"{data.Target.Data.ClassData.name}";
        }
        
        go.transform.Find("Text").GetComponent<TMP_Text>().text = text;
        
        StartCoroutine(ScrollToTop());
    }

    private IEnumerator ScrollToTop()
    {
        yield return null;
        
        // Set scroll to bottom.
        _combatLogScrollRect.verticalNormalizedPosition = 1;
    }

    private void PlaceCharacterStarted()
    {
        CharacterData c = GlobalGameManager.GetInstance().GetGameData().heroDataList[0];
        UpdateCharacterPortraits();
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
        _placeCharactersPanel.SetActive(true);
        
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

    private void SetEndTurnButtonInteractable()
    {
        _endTurnButton.interactable = true;
    }

    private void SetEndTurnButtonUninteractable()
    {
        _endTurnButton.interactable = false;
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
            PortraitButton pb = CreateCharacterPortrait(CombatManager._instance.GetCharacterDataDict()[c], _characterPortraitPanel.transform);
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

        for (int i = 0; i < _characterPortraitPanel.transform.childCount; i++)
        {
            Destroy(_characterPortraitPanel.transform.GetChild(i).gameObject);
        }
    }

    private PortraitButton CreateCharacterPortrait(Character c, Transform parent)
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
            PortraitButton pb = CreateCharacterPortrait(c, _turnOrderPanel.transform);
            _characterPortraits.TryAdd(pb.Character.Data, pb);
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
        
        _abilityPanelParent.SetActive(false);
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
        
        // _abilityPanel.gameObject.SetActive(true);
        _abilityPanelParent.SetActive(true);

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
        if (moving)
        {
            foreach (var abilityButton in _abilityButtons)
            {
                abilityButton.Button.interactable = false;
            }
        }
        else
        {
            UpdateAbilityColors();
        }
    }
}
