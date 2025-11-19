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
    [SerializeField] private GameObject _hand;
    [SerializeField] private TextMeshProUGUI _mana;

    // Colors.
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    
    private void OnEnable()
    {
        CardHandManager.onManaChange += UpdateManaText;
        CombatEventManager.OnEnterCombatStateTakeTurn += UpdateSelectedPortrait;
    }

    private void OnDisable()
    {
        CardHandManager.onManaChange -= UpdateManaText;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _hand.SetActive(false);
        UpdateCharacterPortraits();
        
        // Player 1 portrait displayed as default when no character has been selected yet.
        // UpdateSelectedPortrait(GlobalGameManager.GetInstance().GetGameData().heroDataList[0]);
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
        CardHandManager._instance.OpenDeck();
    }

    public void ShowDiscardPile()
    {
        CardHandManager._instance.OpenDiscardPile();
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
        
        foreach (CharacterData c in heroList)
        {
            Button button = Instantiate(_characterPortraitButtonPrefab, _characterPortraitPanel.transform);
            button.image.sprite = c.ClassData.classImage;
            button.GetComponent<PortraitButton>().SetCharacter(c);
            button.image.color = _inactiveColor;
        }
    }

    public void UpdatePortraitColors(GameObject selectedPortrait)
    {
        selectedPortrait.GetComponent<Image>().color = _activeColor;
        for (int i = 0; i < _characterPortraitPanel.transform.childCount; i++)
        {
            GameObject child = _characterPortraitPanel.transform.GetChild(i).gameObject;
            if (child != selectedPortrait)
            {
                child.GetComponent<Image>().color = _inactiveColor;
            }
        }
    }

    private void UpdateManaText(int mana)
    {
        _mana.text = $"Mana\n{mana}/10";
    }

    private void UpdateSelectedPortrait(Character c)
    {
        if (!c || c.Data.Faction == Faction.Enemy)
        {
            return;
        }
        _activeCharacterPortrait.sprite = c.Data.ClassData.classImage;
    }
    
    public void SetCardsActive(bool active)
    {
        CardHandManager._instance.SetUIActive(active);
        _abilityPanel.color = active ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// Displays each available ability for the selected character.
    /// </summary>
    /// <param name="character">The character of which's abilities to display.</param>
    public void LoadAbilities(CharacterData character)
    {
        if (character == null)
        {
            DebugLog.JoppaLog("No selected character");
            return;
        }

        // Remove all current buttons.
        for (int i = 0; i < _abilityPanel.transform.childCount; i++)
        {
            Destroy(_abilityPanel.transform.GetChild(i).gameObject);
        }
        
        DebugLog.JoppaLog($"Number of abilities: {character.AvailableAbilities.Count}");

        for (int i = 0; i < character.AvailableAbilities.Count; i++)
        {
            Button abilityButton = Instantiate(_abilityButtonPrefab, _abilityPanel.transform);
            
            var ability = character.AvailableAbilities[i];
            // abilityButton.GetComponentInChildren<TextMeshProUGUI>().text = ability.name;
            abilityButton.image.sprite = ability.GetIcon();
            abilityButton.GetComponent<AbilityButton>().SetAbility(ability);
        }
    }
}
