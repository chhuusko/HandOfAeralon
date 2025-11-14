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
    [SerializeField] private Button _startCombatButton;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Button _abilityButtonPrefab;
    [SerializeField] private Button _characterPortraitButtonPrefab;
    [SerializeField] private GameObject _hand;
    [SerializeField] private TextMeshProUGUI _mana;

    private void OnEnable()
    {
        CardHandManager.onManaChange += UpdateManaText;
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
    }

    private void UpdateCharacterPortraits()
    {
        GameData gameData = GlobalGameManager.GetInstance().GetGameData();
        List<Character> heroList = gameData.heroList;

        if (heroList == null)
        {
            DebugLog.JoppaLog("No HeroList");
            return;
        }
        
        foreach (Character c in heroList)
        {
            DebugLog.JoppaLog($"Generating portrait for: {c.name}");
            Button characterPortraitButton = Instantiate(_characterPortraitButtonPrefab, _characterPortraitPanel.transform);
            characterPortraitButton.image.sprite = c.GetClassData().classImage;
        }
    }

    private void UpdateManaText(int mana)
    {
        _mana.text = $"Mana\n{mana}/10";
    }

    public void StartCombat()
    {
        OnStartCombatButtonPressed?.Invoke();
        _startCombatButton.gameObject.SetActive(false);
        _endTurnButton.gameObject.SetActive(true);
        _hand.SetActive(true);
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

    public void SetCardsActive(bool active)
    {
        CardHandManager._instance.SetUIActive(active);
        _abilityPanel.color = active ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }

    public void LoadAbilities(Character character)
    {
        Debug.Log("Loading Abilities");

        if (character == null)
        {
            DebugLog.JoppaLog("No selected character");
            return;
        }
        
        DebugLog.JoppaLog($"Number of abilities: {character.GetAvailableAbilities().Count}");

        for (int i = 0; i < character.GetAvailableAbilities().Count; i++)
        {
            Button abilityButton = Instantiate(_abilityButtonPrefab, _abilityPanel.transform);
            
            var ability = character.GetAvailableAbilities()[i];
            abilityButton.GetComponentInChildren<TextMeshProUGUI>().text = ability.name;
            abilityButton.image.sprite = ability.GetIcon();
        }
    }
}
