using System;
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
    
    [SerializeField] private Button _startCombatButton;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Button _abilityButtonPrefab;
    [SerializeField] private Button _characterPortraitButtonPrefab;
    
    [SerializeField] private GameObject _turnOrderPanel;
    [SerializeField] private GameObject _hand;
    
    [SerializeField] private TextMeshProUGUI _mana;

    // Colors.
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    
    private CharacterData _selectedCharacter;
    private bool _combatStarted;
    
    private List<PortraitButton> _portraitButtons = new();
    private List<AbilityButton> _abilityButtons = new();
    
    private Dictionary<CharacterData, PortraitButton> _characterPortraits = new();
    
    private Character _currentTurnCharacter;
    
    private void OnEnable()
    {
        CardHandManager.onManaChange += UpdateManaText;
        CombatEventManager.OnEnterCombatStateLoadNextLevel += PlaceCharacterStarted;
        CombatEventManager.OnEnterCombatStateTakeTurn += StartTurn;
        CombatEventManager.OnTurnOrderChanged += UpdateTurnOrder;
    }

    private void OnDisable()
    {
        CardHandManager.onManaChange -= UpdateManaText;
        CombatEventManager.OnEnterCombatStatePlaceCharacter -= PlaceCharacterStarted;
        CombatEventManager.OnEnterCombatStateTakeTurn -= StartTurn;
        CombatEventManager.OnTurnOrderChanged -= UpdateTurnOrder;

        foreach (var pb in _portraitButtons)
        {
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
            //DontDestroyOnLoad(gameObject);
            
            _hand.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Player 1 portrait displayed as default when no character has been selected yet.
        // UpdateSelectedPortrait(GlobalGameManager.GetInstance().GetGameData().heroDataList[0]);
    }

    private void Start()
    {
        _hand.SetActive(false);
        UpdateCharacterPortraits();
        UpdateManaText(CardHandManager.GetInstance().GetMana());
    }

    public void StartCombat()
    {
        OnStartCombatButtonPressed?.Invoke();
        
        _startCombatButton.gameObject.SetActive(false);
        _endTurnButton.gameObject.SetActive(true);
        _abilityPanel.gameObject.SetActive(true);
        _hand.SetActive(true);
        
        // Set abilities for first character.
        LoadAbilities(Selector._instance.GetSelectedCharacter()?.Data);
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
        CharacterData c = GlobalGameManager.GetInstance().GetGameData().heroDataList[0];
        UpdateActivePortrait(c);
        UpdatePortraitColors(_characterPortraits[c]);
        LoadAbilities(c);
        
        _selectedCharacter = c;
        _currentTurnCharacter = CombatManager._instance.GetCharacterDataDict()[_selectedCharacter];
    }

    private void StartTurn(Character c)
    {
        UpdateActivePortrait(c);
        UpdatePortraitColors(c);
        
        _selectedCharacter = c.Data;
        _combatStarted = true;
        _currentTurnCharacter = c;
        
        LoadAbilities(_selectedCharacter);
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
        button.image.color = _inactiveColor;
        PortraitButton pb = button.GetComponent<PortraitButton>();
        pb.Character = c;
        
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
    }
    
    public void UpdatePortraitColors(Character c) 
    {
        UpdatePortraitColors(_characterPortraits[c.Data]);
    }

    private void UpdatePortraitColors(PortraitButton selectedPortrait)
    {
        foreach (var pb in _portraitButtons)
        {
            pb.GetComponent<Image>().color = _inactiveColor;
        }
        if(selectedPortrait)
            selectedPortrait.GetComponent<Image>().color = _activeColor;
    }

    private void UpdateManaText(int mana)
    {
        _mana.text = $"Mana\n{mana}/{CardHandManager.GetInstance().GetMaxMana()}";
    }
    
    private void UpdateActivePortrait(PortraitButton pb)
    {
        UpdateActivePortrait(pb.Character);
    }

    public void UpdateActivePortrait(Character c)
    {
        if (!c)
        {
            DebugLog.JoppaLog("Null character");
            return;
        }
        UpdateActivePortrait(c.Data);
    }

    private void UpdateActivePortrait(CharacterData c)
    {
        if (c.Faction == Faction.Enemy)
        {
            DebugLog.JoppaLog("Enemy");
            return;
        }
        DebugLog.JoppaLog("Called");
        _activeCharacterPortrait.sprite = c.ClassData.classImage;
    }
    
    public void SetCardsActive(bool active)
    {
        CardHandManager.GetInstance().SetUIActive(active);
        _abilityPanel.color = active ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// Displays each available ability for the selected character.
    /// </summary>
    /// <param name="portraitButton">The character of which's abilities to display.</param>
    private void LoadAbilities(PortraitButton portraitButton)
    {
        LoadAbilities(portraitButton.Character);
    }
    
    public void LoadAbilities(CharacterData character)
    {
        if (character == null)
        {
            DebugLog.JoppaLog("No selected character");
            return;
        }

        // Don't show abilities for enemies.
        if (character.Faction == Faction.Enemy)
        {
            return;
        }

        if (!_combatStarted)
        {
            return;
        }

        _abilityButtons.Clear();
        // Remove all current ability buttons.
        for (int i = 0; i < _abilityPanel.transform.childCount; i++)
        {
            Destroy(_abilityPanel.transform.GetChild(i).gameObject);
        }
        
        DebugLog.JoppaLog($"Number of abilities: {character.AvailableAbilities.Count}");

        for (int i = 0; i < character.AvailableAbilities.Count; i++)
        {
            Button button = Instantiate(_abilityButtonPrefab, _abilityPanel.transform);
            
            var ability = character.AvailableAbilities[i];
            button.image.sprite = ability.GetIcon();
            button.GetComponent<AbilityButton>().Ability = ability;
            
            var abilityButton = button.GetComponent<AbilityButton>();
            _abilityButtons.Add(abilityButton);
            
            UpdateAbilityColors(CombatManager._instance.GetCharacterDataDict()[character], abilityButton);
        }
    }

    private void UpdateAbilityColors(Character c, AbilityButton abilityButton)
    {
        if (!_currentTurnCharacter)
        {
            abilityButton.Button.interactable = false;
            return;
        }
        
        abilityButton.Button.interactable = !c.IsAbilityCooldownActive(abilityButton.Ability)
                                            && _selectedCharacter.Faction == Faction.Friendly 
                                            && c == _currentTurnCharacter && _combatStarted;
    }
}
